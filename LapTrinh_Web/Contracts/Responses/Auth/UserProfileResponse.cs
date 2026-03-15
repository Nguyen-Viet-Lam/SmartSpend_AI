namespace LapTrinh_Web.Contracts.Responses.Auth;

public sealed class UserProfileResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
}