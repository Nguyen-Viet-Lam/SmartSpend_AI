namespace SmartSpendAI.Models.Dtos.Finance
{
    public class TransactionResponse
    {
        public int TransactionId { get; set; }

        public int WalletId { get; set; }

        public string WalletName { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string Note { get; set; } = string.Empty;

        public DateTime TransactionDate { get; set; }

        public decimal AiConfidence { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
