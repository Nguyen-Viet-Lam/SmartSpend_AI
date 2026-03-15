using LapTrinh_Web.Core.Enums;

namespace LapTrinh_Web.Contracts.Responses.Transactions;

public sealed class TransactionResponse
{
    public Guid TransactionId { get; set; }
    public Guid WalletId { get; set; }
    public Guid? CategoryId { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime OccurredAtUtc { get; set; }
}