using LapTrinh_Web.Core.Enums;

namespace LapTrinh_Web.Contracts.Requests.Transactions;

public sealed class CreateTransactionRequest
{
    public Guid WalletId { get; set; }
    public Guid? CategoryId { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime OccurredAtUtc { get; set; }
}