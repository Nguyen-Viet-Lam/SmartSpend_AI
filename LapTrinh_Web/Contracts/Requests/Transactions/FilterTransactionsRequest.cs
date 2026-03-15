namespace LapTrinh_Web.Contracts.Requests.Transactions;

public sealed class FilterTransactionsRequest
{
    public Guid? WalletId { get; set; }
    public Guid? CategoryId { get; set; }
    public DateTime? FromUtc { get; set; }
    public DateTime? ToUtc { get; set; }
}