using LapTrinh_Web.Contracts.Requests.Auth;
using LapTrinh_Web.Contracts.Responses.Auth;

namespace LapTrinh_Web.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<bool> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken = default);
    Task ResendOtpAsync(ResendOtpRequest request, CancellationToken cancellationToken = default);
    Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default);
}
