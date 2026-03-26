using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartSpendAI.Models;
using SmartSpendAI.Models.Dtos.Auth;
using SmartSpendAI.Security;
using SmartSpendAI.Services.Email;
using SmartSpendAI.Services.Otp;

namespace SmartSpendAI.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly IEmailOtpService _emailOtpService;
        private readonly JwtSettings _jwtSettings;
        private readonly JwtSigningMaterial _jwtSigningMaterial;
        private readonly ILogger<AuthService> _logger;
        private readonly IEmailSender? _emailSender;
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public AuthService(
            AppDbContext dbContext,
            IEmailOtpService emailOtpService,
            IOptions<JwtSettings> jwtSettings,
            JwtSigningMaterial jwtSigningMaterial,
            ILogger<AuthService> logger,
            IEmailSender? emailSender = null,
            IHttpContextAccessor? httpContextAccessor = null)
        {
            _dbContext = dbContext;
            _emailOtpService = emailOtpService;
            _jwtSettings = jwtSettings.Value;
            _jwtSigningMaterial = jwtSigningMaterial;
            _logger = logger;
            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<LoginServiceResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            var identifier = request.EmailOrUsername.Trim();
            var validationErrors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(identifier))
            {
                validationErrors[nameof(LoginRequest.EmailOrUsername)] = ["Email hoac ten dang nhap khong duoc de trong."];
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                validationErrors[nameof(LoginRequest.Password)] = ["Mat khau khong duoc de trong."];
            }

            if (validationErrors.Count > 0)
            {
                return new LoginServiceResult
                {
                    Success = false,
                    StatusCode = 400,
                    ValidationErrors = validationErrors
                };
            }

            var normalizedEmail = identifier.ToLowerInvariant();
            var user = await _dbContext.Users
                .AsNoTracking()
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Email == normalizedEmail || x.Username == identifier, cancellationToken);

            if (user is null || !PasswordHashUtility.VerifyPassword(request.Password, user.PasswordHash))
            {
                return new LoginServiceResult
                {
                    Success = false,
                    StatusCode = 401,
                    Message = "Email/ten dang nhap hoac mat khau khong dung."
                };
            }

            if (user.IsLocked)
            {
                return new LoginServiceResult
                {
                    Success = false,
                    StatusCode = 403,
                    Message = "Tai khoan da bi khoa."
                };
            }

            if (!user.IsEmailVerified)
            {
                return new LoginServiceResult
                {
                    Success = false,
                    StatusCode = 403,
                    Message = "Email chua duoc xac thuc. Vui long nhap OTP truoc khi dang nhap."
                };
            }

            var now = DateTime.UtcNow;
            var lifetime = request.RememberMe
                ? TimeSpan.FromDays(Math.Max(1, _jwtSettings.RememberMeAccessTokenDays))
                : TimeSpan.FromMinutes(Math.Max(1, _jwtSettings.AccessTokenMinutes));
            var expiresAt = now.Add(lifetime);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role?.RoleName ?? AppRoles.StandardUser)
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                notBefore: now,
                expires: expiresAt,
                signingCredentials: _jwtSigningMaterial.CreateSigningCredentials());

            _logger.LogInformation("User logged in successfully UserId={UserId}", user.UserId);
            await TrackLoginAndNotifyIfNewDeviceAsync(user, cancellationToken);

            return new LoginServiceResult
            {
                Success = true,
                StatusCode = 200,
                Response = new LoginResponse
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role?.RoleName ?? AppRoles.StandardUser,
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    ExpiresAt = expiresAt
                }
            };
        }

        public async Task<RegisterServiceResult> RegisterAsync(RegisterRequest request, string requestIp, CancellationToken cancellationToken)
        {
            var username = request.Username.Trim();
            var fullName = request.FullName.Trim();
            var email = request.Email.Trim().ToLowerInvariant();

            var validationErrors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(username))
            {
                validationErrors[nameof(RegisterRequest.Username)] = ["Username khong duoc de trong."];
            }

            if (string.IsNullOrWhiteSpace(fullName))
            {
                validationErrors[nameof(RegisterRequest.FullName)] = ["Ho ten khong duoc de trong."];
            }

            if (!request.AcceptTerms)
            {
                validationErrors[nameof(RegisterRequest.AcceptTerms)] = ["Ban can dong y voi dieu khoan su dung."];
            }

            if (!HasStrongPassword(request.Password))
            {
                validationErrors[nameof(RegisterRequest.Password)] =
                    ["Mat khau can co it nhat 8 ky tu, gom chu hoa, chu thuong va chu so."];
            }

            if (await _dbContext.Users.AnyAsync(x => x.Username == username, cancellationToken))
            {
                validationErrors[nameof(RegisterRequest.Username)] = ["Ten dang nhap da ton tai."];
            }

            if (await _dbContext.Users.AnyAsync(x => x.Email == email, cancellationToken))
            {
                validationErrors[nameof(RegisterRequest.Email)] = ["Email da duoc su dung."];
            }

            if (validationErrors.Count > 0)
            {
                return new RegisterServiceResult
                {
                    Success = false,
                    ValidationErrors = validationErrors
                };
            }

            var user = new User
            {
                Username = username,
                FullName = fullName,
                Email = email,
                PasswordHash = PasswordHashUtility.HashPassword(request.Password),
                RoleId = await EnsureRoleIdAsync(AppRoles.StandardUser, cancellationToken),
                IsLocked = false,
                IsEmailVerified = false,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Users.Add(user);

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                return new RegisterServiceResult
                {
                    Success = false,
                    IsConflict = true,
                    Message = "Ten dang nhap hoac email da ton tai."
                };
            }

            var otpDispatch = await _emailOtpService.IssueRegisterOtpAsync(user, requestIp, cancellationToken);

            return new RegisterServiceResult
            {
                Success = true,
                Response = new RegisterResponse
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    FullName = user.FullName,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt,
                    IsEmailVerified = user.IsEmailVerified,
                    OtpDispatched = otpDispatch.Success,
                    OtpExpiresAt = otpDispatch.ExpiresAt,
                    Message = otpDispatch.Success
                        ? "Dang ky thanh cong. Vui long kiem tra email de nhap OTP xac thuc."
                        : "Dang ky thanh cong nhung chua gui duoc OTP. Vui long gui lai OTP."
                }
            };
        }

        public Task<OtpVerificationResult> VerifyEmailOtpAsync(VerifyEmailOtpRequest request, CancellationToken cancellationToken)
        {
            return _emailOtpService.VerifyRegisterOtpAsync(request.Email, request.OtpCode, cancellationToken);
        }

        public Task<OtpDispatchResult> ResendEmailOtpAsync(ResendEmailOtpRequest request, string requestIp, CancellationToken cancellationToken)
        {
            return _emailOtpService.ResendRegisterOtpAsync(request.Email, requestIp, cancellationToken);
        }

        public async Task<OtpDispatchResult> RequestPasswordResetAsync(
            ForgotPasswordRequest request,
            string requestIp,
            CancellationToken cancellationToken)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == normalizedEmail, cancellationToken);
            if (user is null)
            {
                return new OtpDispatchResult
                {
                    Success = true,
                    Message = "Neu email ton tai, he thong da gui ma OTP dat lai mat khau."
                };
            }

            var dispatch = await _emailOtpService.IssuePasswordResetOtpAsync(user, requestIp, cancellationToken);
            return new OtpDispatchResult
            {
                Success = dispatch.Success,
                Message = dispatch.Success
                    ? "He thong da gui ma OTP dat lai mat khau."
                    : dispatch.Message,
                ExpiresAt = dispatch.ExpiresAt
            };
        }

        public async Task<SimpleServiceResult> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            var validationErrors = new Dictionary<string, string[]>();
            if (!HasStrongPassword(request.NewPassword))
            {
                validationErrors[nameof(ResetPasswordRequest.NewPassword)] =
                    ["Mat khau can co it nhat 8 ky tu, gom chu hoa, chu thuong va chu so."];
            }

            if (validationErrors.Count > 0)
            {
                return new SimpleServiceResult
                {
                    Success = false,
                    ValidationErrors = validationErrors
                };
            }

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == normalizedEmail, cancellationToken);
            if (user is null)
            {
                return new SimpleServiceResult
                {
                    Success = false,
                    Message = "Khong tim thay tai khoan can dat lai mat khau."
                };
            }

            var verify = await _emailOtpService.VerifyPasswordResetOtpAsync(request.Email, request.OtpCode, cancellationToken);
            if (!verify.Success)
            {
                return new SimpleServiceResult
                {
                    Success = false,
                    Message = verify.Message
                };
            }

            user.PasswordHash = PasswordHashUtility.HashPassword(request.NewPassword);
            user.IsEmailVerified = true;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new SimpleServiceResult
            {
                Success = true,
                Message = "Dat lai mat khau thanh cong."
            };
        }

        private async Task<int> EnsureRoleIdAsync(string roleName, CancellationToken cancellationToken)
        {
            var existingRole = await _dbContext.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.RoleName == roleName, cancellationToken);

            if (existingRole is not null)
            {
                return existingRole.RoleId;
            }

            var role = new Role { RoleName = roleName };
            _dbContext.Roles.Add(role);

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
                return role.RoleId;
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                _dbContext.Entry(role).State = EntityState.Detached;
                var fallback = await _dbContext.Roles.AsNoTracking().FirstAsync(x => x.RoleName == roleName, cancellationToken);
                return fallback.RoleId;
            }
        }

        private static bool HasStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                return false;
            }

            var hasUpper = Regex.IsMatch(password, "[A-Z]");
            var hasLower = Regex.IsMatch(password, "[a-z]");
            var hasDigit = Regex.IsMatch(password, "[0-9]");

            return hasUpper && hasLower && hasDigit;
        }

        private static bool IsUniqueConstraintViolation(DbUpdateException exception)
        {
            return exception.InnerException is SqlException sqlException &&
                   (sqlException.Number == 2601 || sqlException.Number == 2627);
        }

        private async Task TrackLoginAndNotifyIfNewDeviceAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                var httpContext = _httpContextAccessor?.HttpContext;
                var requestIp = httpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown-ip";
                var userAgent = httpContext?.Request.Headers.UserAgent.ToString();
                var sanitizedUserAgent = string.IsNullOrWhiteSpace(userAgent) ? "unknown-agent" : userAgent.Trim();
                var deviceFingerprint = BuildDeviceFingerprint(requestIp, sanitizedUserAgent);

                var previousLogin = await _dbContext.AuditLogs
                    .AsNoTracking()
                    .Where(x => x.ActorUserId == user.UserId && x.Action == "UserLoginSuccess")
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync(cancellationToken);

                var isNewDevice = previousLogin is not null &&
                                  !string.Equals(previousLogin.Metadata, deviceFingerprint, StringComparison.Ordinal);

                _dbContext.AuditLogs.Add(new AuditLog
                {
                    ActorUserId = user.UserId,
                    Action = "UserLoginSuccess",
                    TargetType = "User",
                    TargetId = user.UserId.ToString(),
                    Metadata = deviceFingerprint,
                    CreatedAt = DateTime.UtcNow
                });

                if (isNewDevice && _emailSender is not null)
                {
                    var issuedAt = DateTime.UtcNow;
                    var subject = "Canh bao bao mat: Phat hien dang nhap thiet bi la";
                    var textBody =
                        $"He thong phat hien dang nhap moi vao tai khoan {user.Email}.\n" +
                        $"Thoi gian (UTC): {issuedAt:yyyy-MM-dd HH:mm:ss}\n" +
                        $"IP: {requestIp}\n" +
                        $"User-Agent: {sanitizedUserAgent}\n\n" +
                        "Neu day khong phai ban, vui long doi mat khau ngay lap tuc.";

                    var htmlBody =
                        "<p>He thong phat hien <strong>dang nhap moi</strong> vao tai khoan cua ban.</p>" +
                        $"<p><strong>Thoi gian (UTC):</strong> {issuedAt:yyyy-MM-dd HH:mm:ss}<br/>" +
                        $"<strong>IP:</strong> {requestIp}<br/>" +
                        $"<strong>User-Agent:</strong> {sanitizedUserAgent}</p>" +
                        "<p>Neu day khong phai ban, vui long doi mat khau ngay lap tuc.</p>";

                    await _emailSender.SendAsync(user.Email, subject, htmlBody, textBody, cancellationToken);

                    _dbContext.AuditLogs.Add(new AuditLog
                    {
                        ActorUserId = user.UserId,
                        Action = "UserLoginNewDeviceAlertSent",
                        TargetType = "User",
                        TargetId = user.UserId.ToString(),
                        Metadata = deviceFingerprint,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not persist login audit or send suspicious-device alert for UserId={UserId}.", user.UserId);
            }
        }

        private static string BuildDeviceFingerprint(string requestIp, string userAgent)
        {
            return $"ip={requestIp};ua={userAgent}";
        }
    }
}
