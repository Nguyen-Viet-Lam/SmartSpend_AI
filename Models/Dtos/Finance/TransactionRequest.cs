using System.ComponentModel.DataAnnotations;

namespace SmartSpendAI.Models.Dtos.Finance
{
    public class TransactionRequest
    {
        [Required]
        public int WalletId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(32)]
        public string Type { get; set; } = "Expense";

        [Range(0.01, 999999999)]
        public decimal Amount { get; set; }

        [MaxLength(400)]
        public string Note { get; set; } = string.Empty;

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        [MaxLength(260)]
        public string? ReceiptImagePath { get; set; }
    }
}
