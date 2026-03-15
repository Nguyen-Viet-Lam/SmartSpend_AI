using LapTrinh_Web.Contracts.Requests.Transactions;
using LapTrinh_Web.Contracts.Responses.Transactions;

namespace LapTrinh_Web.Services.Interfaces;

public interface ITransactionService
{
    Task<IReadOnlyList<TransactionResponse>> GetByFilterAsync(Guid userId, FilterTransactionsRequest request, CancellationToken cancellationToken = default);
    Task<TransactionResponse> CreateAsync(Guid userId, CreateTransactionRequest request, CancellationToken cancellationToken = default);
    Task<TransactionResponse> UpdateAsync(Guid userId, Guid transactionId, UpdateTransactionRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid userId, Guid transactionId, CancellationToken cancellationToken = default);
}