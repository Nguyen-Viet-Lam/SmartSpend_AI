namespace LapTrinh_Web.Contracts.Requests.Auth;

public sealed class ResendOtpRequest
{
    public string Email { get; set; } = string.Empty;
}
