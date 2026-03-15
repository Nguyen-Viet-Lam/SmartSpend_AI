using LapTrinh_Web.Contracts.Requests.Admin;
using LapTrinh_Web.Contracts.Responses.Admin;

namespace LapTrinh_Web.Services.Interfaces;

public interface IAdminService
{
    Task<IReadOnlyList<UserListItemResponse>> GetUsersAsync(CancellationToken cancellationToken = default);
    Task UpdateUserStatusAsync(Guid userId, UpdateUserStatusRequest request, CancellationToken cancellationToken = default);
    Task UpdateCategoryAsync(Guid categoryId, UpdateCategoryRequest request, CancellationToken cancellationToken = default);
}