namespace LapTrinh_Web.Configuration;

public sealed class SecurityOptions
{
    public const string SectionName = "Security";

    public int OtpLength { get; set; } = 6;
    public int OtpExpireMinutes { get; set; } = 5;
}