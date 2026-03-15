using System.Security.Cryptography;
using LapTrinh_Web.Contracts.Requests.Auth;
using LapTrinh_Web.Contracts.Responses.Auth;
using LapTrinh_Web.Core.Entities;
using LapTrinh_Web.Core.Enums;
using LapTrinh_Web.Data;
using LapTrinh_Web.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LapTrinh_Web.Services.Implementations;

public sealed class AuthService(
    AppDbContext dbContext,
    IEmailDispatchQueue emailDispatchQueue,
    IPasswordHasher<ApplicationUser> passwordHasher,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);
        var displayName = request.DisplayName?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Email, mat khau va ten hien thi la bat buoc.");
        }

        var existedUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        if (existedUser is not null)
        {
            throw new InvalidOperationException("Email da duoc dang ky.");
        }

        var user = new ApplicationUser
        {
            Email = email,
            Role = UserRole.User,
            Status = AccountStatus.PendingVerification
        };
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        var profile = new UserProfile
        {
            UserId = user.Id,
            DisplayName = displayName
        };

        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.UserProfiles.AddAsync(profile, cancellationToken);
        await GenerateAndSendOtpAsync(user, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return ToAuthResponse(user, requiresOtpVerification: true);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("Email va mat khau la bat buoc.");
        }

        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        if (user is null)
        {
            throw new UnauthorizedAccessException("Email hoac mat khau khong dung.");
        }

        if (user.Status == AccountStatus.Locked)
        {
            throw new InvalidOperationException("Tai khoan dang bi khoa.");
        }

        var verify = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verify == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Email hoac mat khau khong dung.");
        }

        if (verify == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
        }

        if (user.Status == AccountStatus.PendingVerification)
        {
            await GenerateAndSendOtpAsync(user, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return ToAuthResponse(user, requiresOtpVerification: true);
        }

        user.LastLoginAtUtc = DateTime.UtcNow;
        user.UpdatedAtUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToAuthResponse(user, requiresOtpVerification: false);
    }

    public async Task<bool> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);
        var otpCode = request.OtpCode?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(otpCode))
        {
            return false;
        }

        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        if (user is null)
        {
            return false;
        }

        var now = DateTime.UtcNow;
        var otp = await dbContext.OtpCodes
            .Where(x => x.UserId == user.Id && !x.IsUsed && x.Code == otpCode && x.ExpiredAtUtc >= now)
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (otp is null)
        {
            return false;
        }

        otp.IsUsed = true;
        user.Status = AccountStatus.Active;
        user.LastLoginAtUtc = now;
        user.UpdatedAtUtc = now;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task ResendOtpAsync(ResendOtpRequest request, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email la bat buoc.");
        }

        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        if (user is null)
        {
            throw new InvalidOperationException("Tai khoan khong ton tai.");
        }

        if (user.Status == AccountStatus.Locked)
        {
            throw new InvalidOperationException("Tai khoan dang bi khoa.");
        }

        if (user.Status == AccountStatus.Active)
        {
            throw new InvalidOperationException("Tai khoan da xac thuc.");
        }

        await GenerateAndSendOtpAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null)
        {
            throw new InvalidOperationException("Khong tim thay nguoi dung.");
        }

        var verify = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);
        if (verify == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Mat khau hien tai khong dung.");
        }

        user.PasswordHash = passwordHasher.HashPassword(user, request.NewPassword);
        user.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task GenerateAndSendOtpAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var activeOtps = await dbContext.OtpCodes
            .Where(x => x.UserId == user.Id && !x.IsUsed && x.ExpiredAtUtc >= now)
            .ToListAsync(cancellationToken);

        foreach (var item in activeOtps)
        {
            item.IsUsed = true;
        }

        var otpCode = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        var otp = new OtpCode
        {
            UserId = user.Id,
            Code = otpCode,
            ExpiredAtUtc = now.AddMinutes(5),
            IsUsed = false
        };

        await dbContext.OtpCodes.AddAsync(otp, cancellationToken);

        try
        {
            await emailDispatchQueue.QueueOtpEmailAsync(user.Email, otpCode, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Queue OTP email failed for {Email}", user.Email);
            throw;
        }
    }

    private static string NormalizeEmail(string? email)
        => email?.Trim().ToLowerInvariant() ?? string.Empty;

    private static AuthResponse ToAuthResponse(ApplicationUser user, bool requiresOtpVerification)
    {
        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role,
            RequiresOtpVerification = requiresOtpVerification
        };
    }
}
