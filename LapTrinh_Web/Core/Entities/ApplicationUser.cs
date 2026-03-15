using LapTrinh_Web.Core.Enums;

namespace LapTrinh_Web.Core.Entities;

public class ApplicationUser : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public AccountStatus Status { get; set; } = AccountStatus.PendingVerification;
    public DateTime? LastLoginAtUtc { get; set; }
}