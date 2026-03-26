using SmartSpendAI.Models.Dtos.Auth;
using SmartSpendAI.Services.Otp;

namespace SmartSpendAI.Services.Auth
{
    public interface IAuthService
    {
        Task<LoginServiceResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken);

        Task<RegisterServiceResult> RegisterAsync(RegisterRequest request, string requestIp, CancellationToken cancellationToken);

        Task<OtpVerificationResult> VerifyEmailOtpAsync(VerifyEmailOtpRequest request, CancellationToken cancellationToken);

        Task<OtpDispatchResult> ResendEmailOtpAsync(ResendEmailOtpRequest request, string requestIp, CancellationToken cancellationToken);

        Task<OtpDispatchResult> RequestPasswordResetAsync(ForgotPasswordRequest request, string requestIp, CancellationToken cancellationToken);

        Task<SimpleServiceResult> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken);
    }
}
