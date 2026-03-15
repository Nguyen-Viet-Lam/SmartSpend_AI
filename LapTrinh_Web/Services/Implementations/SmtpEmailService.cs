using System.Net;
using System.Net.Mail;
using LapTrinh_Web.Configuration;
using LapTrinh_Web.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace LapTrinh_Web.Services.Implementations;

public sealed class SmtpEmailService(
    IOptions<SmtpOptions> smtpOptions,
    ILogger<SmtpEmailService> logger) : IEmailService
{
    public async Task SendOtpEmailAsync(string toEmail, string otpCode, CancellationToken cancellationToken = default)
    {
        var options = smtpOptions.Value;
        if (string.IsNullOrWhiteSpace(options.SenderEmail) || string.IsNullOrWhiteSpace(options.AppPassword))
        {
            logger.LogWarning("SMTP config is not complete. OTP for {Email}: {OtpCode}", toEmail, otpCode);
            return;
        }

        using var message = new MailMessage
        {
            From = new MailAddress(options.SenderEmail, options.SenderName),
            Subject = "[SmartSpend AI] Ma OTP xac thuc tai khoan",
            IsBodyHtml = true,
            Body = BuildOtpHtml(otpCode)
        };
        message.To.Add(toEmail);

        using var client = new SmtpClient(options.Host, options.Port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(options.SenderEmail, options.AppPassword)
        };

        cancellationToken.ThrowIfCancellationRequested();
        await client.SendMailAsync(message);
    }

    private static string BuildOtpHtml(string otpCode)
    {
        return $"""
                <div style="font-family:Segoe UI,Arial,sans-serif;line-height:1.6;color:#1f2937">
                    <h2 style="margin:0 0 8px;color:#0f4c81">SmartSpend AI</h2>
                    <p>Ma OTP cua ban la:</p>
                    <div style="font-size:28px;font-weight:700;letter-spacing:3px;padding:8px 12px;background:#f3f4f6;border-radius:8px;display:inline-block">
                        {WebUtility.HtmlEncode(otpCode)}
                    </div>
                    <p style="margin-top:12px">Ma co hieu luc trong vai phut. Khong chia se ma nay voi nguoi khac.</p>
                </div>
                """;
    }
}
