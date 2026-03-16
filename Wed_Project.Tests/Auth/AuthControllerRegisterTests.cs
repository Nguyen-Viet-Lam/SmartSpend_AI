using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web_Project.Controllers;
using Web_Project.Models;
using Web_Project.Services.Auth;
using Web_Project.Services.Otp;

namespace Web_Project.Tests.Auth;

public sealed class AuthControllerRegisterTests
{
    [Fact]
    public async Task Register_ReturnsCreated_WhenServiceSucceeds()
    {
        var authService = new StubAuthService();
        var controller = CreateController(authService);

        var actionResult = await controller.Register(CreateValidRequest(), CancellationToken.None);

        var created = Assert.IsType<CreatedResult>(actionResult.Result);
        Assert.Equal(StatusCodes.Status201Created, created.StatusCode);
        Assert.Equal("/api/auth/users/42", created.Location);
    }

    [Fact]
    public async Task RequestPasswordReset_ReturnsOk_WhenServiceSucceeds()
    {
        var authService = new StubAuthService();
        var controller = CreateController(authService);

        var actionResult = await controller.RequestPasswordReset(
            new ForgotPasswordRequest { Email = "new.user@example.com" },
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
    }

    private static AuthController CreateController(StubAuthService authService, string? remoteIp = "203.0.113.10")
    {
        var httpContext = new DefaultHttpContext();
        if (!string.IsNullOrWhiteSpace(remoteIp))
        {
            httpContext.Connection.RemoteIpAddress = IPAddress.Parse(remoteIp);
        }

        return new AuthController(authService)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };
    }

    private static RegisterRequest CreateValidRequest()
    {
        return new RegisterRequest
        {
            Username = "new.user",
            FullName = "New User",
            Email = "new.user@example.com",
            Password = "StrongPass1",
            ConfirmPassword = "StrongPass1",
            AcceptTerms = true
        };
    }

    private sealed class StubAuthService : IAuthService
    {
        public Task<LoginServiceResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new LoginServiceResult
            {
                Success = true,
                Response = new LoginResponse
                {
                    UserId = 1,
                    Username = "new.user",
                    FullName = "New User",
                    Email = "new.user@example.com",
                    Role = "StandardUser",
                    AccessToken = "token"
                }
            });
        }

        public Task<RegisterServiceResult> RegisterAsync(RegisterRequest request, string requestIp, CancellationToken cancellationToken)
        {
            return Task.FromResult(new RegisterServiceResult
            {
                Success = true,
                Response = new RegisterResponse
                {
                    UserId = 42,
                    Username = request.Username,
                    FullName = request.FullName,
                    Email = request.Email,
                    OtpDispatched = true
                }
            });
        }

        public Task<OtpVerificationResult> VerifyEmailOtpAsync(VerifyEmailOtpRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new OtpVerificationResult { Success = true });
        }

        public Task<OtpDispatchResult> ResendEmailOtpAsync(ResendEmailOtpRequest request, string requestIp, CancellationToken cancellationToken)
        {
            return Task.FromResult(new OtpDispatchResult { Success = true });
        }

        public Task<OtpDispatchResult> RequestPasswordResetAsync(ForgotPasswordRequest request, string requestIp, CancellationToken cancellationToken)
        {
            return Task.FromResult(new OtpDispatchResult { Success = true, Message = "ok" });
        }

        public Task<SimpleServiceResult> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new SimpleServiceResult { Success = true, Message = "ok" });
        }
    }
}
