using LapTrinh_Web.Contracts.Requests.Admin;
using LapTrinh_Web.Contracts.Responses.Admin;
using LapTrinh_Web.Services.Interfaces;

namespace LapTrinh_Web.Services.Implementations;

public sealed class AdminService : IAdminService
{
    public Task<IReadOnlyList<UserListItemResponse>> GetUsersAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task UpdateUserStatusAsync(Guid userId, UpdateUserStatusRequest request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task UpdateCategoryAsync(Guid categoryId, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}