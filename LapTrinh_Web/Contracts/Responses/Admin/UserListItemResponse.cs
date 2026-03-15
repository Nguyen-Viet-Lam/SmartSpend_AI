using LapTrinh_Web.Core.Enums;

namespace LapTrinh_Web.Contracts.Responses.Admin;

public sealed class UserListItemResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public AccountStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}