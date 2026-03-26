using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SmartSpendAI.Models;
using SmartSpendAI.Models.Dtos.Auth;
using SmartSpendAI.Security;
using SmartSpendAI.Services.Auth;
using SmartSpendAI.Services.Email;
using SmartSpendAI.Services.Otp;

namespace SmartSpendAI.Tests.Auth;

public sealed class AuthOtpFlowTests
{
    [Fact]
    public async Task VerifyRegisterOtpAsync_MarksUserAsVerified_WhenOtpIsValid()
    {
        using var dbContext = CreateDbContext();
        var user = await SeedUserAsync(dbContext, "otp-user@example.com", "StrongPass1", isEmailVerified: false);
        await SeedRegisterOtpAsync(dbContext, user, "123456", DateTime.UtcNow.AddMinutes(5));

        var otpService = CreateOtpService(dbContext);

        var result = await otpService.VerifyRegisterOtpAsync(user.Email, "123456", CancellationToken.None);

        Assert.True(result.Success);

        var savedUser = await dbContext.Users.AsNoTracking().SingleAsync(x => x.UserId == user.UserId);
        var savedOtp = await dbContext.EmailVerificationOtps.AsNoTracking().SingleAsync(x => x.UserId == user.UserId);
        Assert.True(savedUser.IsEmailVerified);
        Assert.True(savedOtp.IsUsed);
        Assert.NotNull(savedOtp.UsedAt);
    }

    [Fact]
    public async Task LoginAsync_BlocksUnverifiedUser_And_AllowsAfterOtpVerification()
    {
        using var dbContext = CreateDbContext();
        var user = await SeedUserAsync(dbContext, "login-flow@example.com", "StrongPass1", isEmailVerified: false);
        await SeedRegisterOtpAsync(dbContext, user, "654321", DateTime.UtcNow.AddMinutes(5));

        var authService = CreateAuthService(dbContext);

        var blockedResult = await authService.LoginAsync(
            new LoginRequest
            {
                EmailOrUsername = user.Email,
                Password = "StrongPass1",
                RememberMe = false
            },
            CancellationToken.None);

        Assert.False(blockedResult.Success);
        Assert.Equal(403, blockedResult.StatusCode);

        var otpService = CreateOtpService(dbContext);
        var verifyResult = await otpService.VerifyRegisterOtpAsync(user.Email, "654321", CancellationToken.None);
        Assert.True(verifyResult.Success);

        var allowedResult = await authService.LoginAsync(
            new LoginRequest
            {
                EmailOrUsername = user.Email,
                Password = "StrongPass1",
                RememberMe = false
            },
            CancellationToken.None);

        Assert.True(allowedResult.Success);
        Assert.Equal(200, allowedResult.StatusCode);
        Assert.NotNull(allowedResult.Response);
        Assert.Equal(user.Email, allowedResult.Response!.Email);
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"otp-flow-tests-{Guid.NewGuid()}")
            .Options;

        return new AppDbContext(options);
    }

    private static async Task<User> SeedUserAsync(
        AppDbContext dbContext,
        string email,
        string password,
        bool isEmailVerified)
    {
        var role = new Role { RoleId = 1, RoleName = AppRoles.StandardUser };
        dbContext.Roles.Add(role);

        var user = new User
        {
            Username = email.Split('@')[0],
            FullName = "OTP Flow User",
            Email = email.ToLowerInvariant(),
            PasswordHash = PasswordHashUtility.HashPassword(password),
            RoleId = role.RoleId,
            IsLocked = false,
            IsEmailVerified = isEmailVerified,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        return user;
    }

    private static async Task SeedRegisterOtpAsync(AppDbContext dbContext, User user, string otpCode, DateTime expiresAtUtc)
    {
        var salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
        var hash = HashOtp(otpCode, salt);

        dbContext.EmailVerificationOtps.Add(new EmailVerificationOtp
        {
            Email = user.Email,
            Purpose = OtpPurposes.Register,
            UserId = user.UserId,
            OtpHash = hash,
            OtpSalt = salt,
            AttemptCount = 0,
            IsUsed = false,
            RequestedIp = "127.0.0.1",
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAtUtc
        });

        await dbContext.SaveChangesAsync();
    }

    private static EmailOtpService CreateOtpService(AppDbContext dbContext)
    {
        return new EmailOtpService(
            dbContext,
            new FakeEmailSender(),
            Options.Create(new EmailOtpSettings { ExpireMinutes = 10, MaxAttempts = 5 }),
            NullLogger<EmailOtpService>.Instance);
    }

    private static AuthService CreateAuthService(AppDbContext dbContext)
    {
        var jwtSettings = new JwtSettings
        {
            Issuer = "SmartSpendAI.Tests",
            Audience = "SmartSpendAI.Tests.Client",
            SecretKey = "this-is-a-very-strong-test-secret-key-12345",
            AccessTokenMinutes = 60,
            RememberMeAccessTokenDays = 7
        };

        var signingMaterial = JwtSigningMaterial.Create(jwtSettings, Directory.GetCurrentDirectory());
        return new AuthService(
            dbContext,
            new NoopOtpService(),
            Options.Create(jwtSettings),
            signingMaterial,
            NullLogger<AuthService>.Instance);
    }

    private static string HashOtp(string otpCode, string salt)
    {
        var input = Encoding.UTF8.GetBytes($"{otpCode}:{salt}");
        return Convert.ToBase64String(SHA256.HashData(input));
    }

    private sealed class FakeEmailSender : IEmailSender
    {
        public Task SendAsync(string toEmail, string subject, string htmlBody, string textBody, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class NoopOtpService : IEmailOtpService
    {
        public Task<OtpDispatchResult> IssueRegisterOtpAsync(User user, string requestIp, CancellationToken cancellationToken)
        {
            return Task.FromResult(new OtpDispatchResult { Success = true });
        }

        public Task<OtpVerificationResult> VerifyRegisterOtpAsync(string email, string otpCode, CancellationToken cancellationToken)
        {
            return Task.FromResult(new OtpVerificationResult { Success = true });
        }

        public Task<OtpDispatchResult> ResendRegisterOtpAsync(string email, string requestIp, CancellationToken cancellationToken)
        {
            return Task.FromResult(new OtpDispatchResult { Success = true });
        }

        public Task<OtpDispatchResult> IssuePasswordResetOtpAsync(User user, string requestIp, CancellationToken cancellationToken)
        {
            return Task.FromResult(new OtpDispatchResult { Success = true });
        }

        public Task<OtpVerificationResult> VerifyPasswordResetOtpAsync(string email, string otpCode, CancellationToken cancellationToken)
        {
            return Task.FromResult(new OtpVerificationResult { Success = true });
        }
    }
}
