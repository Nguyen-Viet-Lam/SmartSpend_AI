namespace LapTrinh_Web.Configuration;

public sealed class SmtpOptions
{
    public const string SectionName = "Smtp";

    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = "SmartSpend AI";
    public string AppPassword { get; set; } = string.Empty;
}