using LapTrinh_Web.Contracts.Requests.Transactions;
using LapTrinh_Web.Contracts.Responses.Transactions;
using LapTrinh_Web.Services.Interfaces;

namespace LapTrinh_Web.Services.Implementations;

public sealed class TransactionService : ITransactionService
{
    public Task<IReadOnlyList<TransactionResponse>> GetByFilterAsync(Guid userId, FilterTransactionsRequest request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<TransactionResponse> CreateAsync(Guid userId, CreateTransactionRequest request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<TransactionResponse> UpdateAsync(Guid userId, Guid transactionId, UpdateTransactionRequest request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task DeleteAsync(Guid userId, Guid transactionId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}