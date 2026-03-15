using LapTrinh_Web.Contracts.Requests.Wallets;
using LapTrinh_Web.Contracts.Responses.Wallets;
using LapTrinh_Web.Services.Interfaces;

namespace LapTrinh_Web.Services.Implementations;

public sealed class WalletService : IWalletService
{
    public Task<IReadOnlyList<WalletResponse>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<WalletResponse> CreateAsync(Guid userId, CreateWalletRequest request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<WalletResponse> UpdateAsync(Guid userId, Guid walletId, UpdateWalletRequest request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task DeleteAsync(Guid userId, Guid walletId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}