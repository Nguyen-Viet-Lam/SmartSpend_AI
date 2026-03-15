using LapTrinh_Web.Core.Enums;

namespace LapTrinh_Web.Contracts.Responses.Auth;

public sealed class AuthResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public bool RequiresOtpVerification { get; set; }
}