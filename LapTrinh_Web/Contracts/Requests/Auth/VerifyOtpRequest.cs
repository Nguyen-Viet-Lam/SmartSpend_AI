namespace LapTrinh_Web.Contracts.Requests.Auth;

public sealed class VerifyOtpRequest
{
    public string Email { get; set; } = string.Empty;
    public string OtpCode { get; set; } = string.Empty;
}