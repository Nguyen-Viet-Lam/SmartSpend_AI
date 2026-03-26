using SmartSpendAI.Models;

namespace SmartSpendAI.Services.Otp
{
    public interface IEmailOtpService
    {
        Task<OtpDispatchResult> IssueRegisterOtpAsync(User user, string requestIp, CancellationToken cancellationToken);

        Task<OtpVerificationResult> VerifyRegisterOtpAsync(string email, string otpCode, CancellationToken cancellationToken);

        Task<OtpDispatchResult> ResendRegisterOtpAsync(string email, string requestIp, CancellationToken cancellationToken);

        Task<OtpDispatchResult> IssuePasswordResetOtpAsync(User user, string requestIp, CancellationToken cancellationToken);

        Task<OtpVerificationResult> VerifyPasswordResetOtpAsync(string email, string otpCode, CancellationToken cancellationToken);
    }
}
