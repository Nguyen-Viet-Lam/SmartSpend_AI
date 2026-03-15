namespace LapTrinh_Web.Core.Entities;

public class OtpCode : BaseEntity
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime ExpiredAtUtc { get; set; }
    public bool IsUsed { get; set; }
}