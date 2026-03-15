using LapTrinh_Web.Contracts.Requests.Wallets;
using LapTrinh_Web.Contracts.Responses.Wallets;

namespace LapTrinh_Web.Services.Interfaces;

public interface IWalletService
{
    Task<IReadOnlyList<WalletResponse>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<WalletResponse> CreateAsync(Guid userId, CreateWalletRequest request, CancellationToken cancellationToken = default);
    Task<WalletResponse> UpdateAsync(Guid userId, Guid walletId, UpdateWalletRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid userId, Guid walletId, CancellationToken cancellationToken = default);
}