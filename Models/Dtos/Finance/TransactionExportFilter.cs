namespace SmartSpendAI.Models.Dtos.Finance
{
    public class TransactionExportFilter
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public int? WalletId { get; set; }

        public int? CategoryId { get; set; }

        public string? Type { get; set; }
    }
}
