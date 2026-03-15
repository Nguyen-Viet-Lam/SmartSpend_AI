using LapTrinh_Web.Contracts.Responses.Auth;

namespace LapTrinh_Web.Services.Interfaces;

public interface IUserProfileService
{
    Task<UserProfileResponse> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserProfileResponse> UpdateAvatarAsync(Guid userId, string avatarUrl, CancellationToken cancellationToken = default);
}