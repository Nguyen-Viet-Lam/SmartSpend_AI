using LapTrinh_Web.Services.Models;

namespace LapTrinh_Web.Services.Interfaces;

public interface IEmailDispatchQueue
{
    ValueTask QueueOtpEmailAsync(string toEmail, string otpCode, CancellationToken cancellationToken = default);
    IAsyncEnumerable<EmailDispatchItem> DequeueAllAsync(CancellationToken cancellationToken = default);
}
