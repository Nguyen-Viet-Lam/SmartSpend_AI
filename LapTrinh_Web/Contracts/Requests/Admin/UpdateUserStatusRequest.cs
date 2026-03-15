using LapTrinh_Web.Core.Enums;

namespace LapTrinh_Web.Contracts.Requests.Admin;

public sealed class UpdateUserStatusRequest
{
    public AccountStatus Status { get; set; }
}