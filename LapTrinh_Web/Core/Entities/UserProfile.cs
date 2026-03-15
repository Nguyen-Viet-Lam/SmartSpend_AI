namespace LapTrinh_Web.Core.Entities;

public class UserProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
}