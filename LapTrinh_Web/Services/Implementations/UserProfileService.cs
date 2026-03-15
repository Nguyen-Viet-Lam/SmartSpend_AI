using LapTrinh_Web.Contracts.Responses.Auth;
using LapTrinh_Web.Services.Interfaces;

namespace LapTrinh_Web.Services.Implementations;

public sealed class UserProfileService : IUserProfileService
{
    public Task<UserProfileResponse> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<UserProfileResponse> UpdateAvatarAsync(Guid userId, string avatarUrl, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}