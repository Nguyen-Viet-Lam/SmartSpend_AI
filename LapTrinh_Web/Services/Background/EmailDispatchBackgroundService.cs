using LapTrinh_Web.Services.Interfaces;

namespace LapTrinh_Web.Services.Background;

public sealed class EmailDispatchBackgroundService(
    IEmailDispatchQueue emailDispatchQueue,
    IEmailService emailService,
    ILogger<EmailDispatchBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var item in emailDispatchQueue.DequeueAllAsync(stoppingToken))
        {
            try
            {
                await emailService.SendOtpEmailAsync(item.ToEmail, item.OtpCode, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Send OTP email failed in background worker for {Email}", item.ToEmail);
            }
        }
    }
}
