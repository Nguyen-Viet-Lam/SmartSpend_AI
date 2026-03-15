namespace LapTrinh_Web.Services.Interfaces;

public interface IEmailService
{
    Task SendOtpEmailAsync(string toEmail, string otpCode, CancellationToken cancellationToken = default);
}
