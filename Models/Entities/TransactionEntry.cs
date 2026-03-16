using System.ComponentModel.DataAnnotations;

namespace Web_Project.Models
{
    public class TransactionEntry
    {
        [Key]
        public int TransactionEntryId { get; set; }

        public int UserId { get; set; }

        public int WalletId { get; set; }

        public int CategoryId { get; set; }

        [Required]
        [MaxLength(32)]
        public string Type { get; set; } = "Expense";

        public decimal Amount { get; set; }

        [MaxLength(400)]
        public string Note { get; set; } = string.Empty;

        public DateTime TransactionDate { get; set; }

        [MaxLength(260)]
        public string? ReceiptImagePath { get; set; }

        public decimal AiConfidence { get; set; }

        public DateTime CreatedAt { get; set; }

        public User User { get; set; } = null!;

        public Wallet Wallet { get; set; } = null!;

        public Category Category { get; set; } = null!;
    }
}
