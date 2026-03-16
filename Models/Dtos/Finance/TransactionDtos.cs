using System.ComponentModel.DataAnnotations;

namespace Web_Project.Models.Dtos.Finance
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

    public class SmartInputRequest
    {
        [Required]
        [MaxLength(300)]
        public string Input { get; set; } = string.Empty;
    }

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

    public class SmartInputResponse
    {
        public decimal Amount { get; set; }

        public int? SuggestedCategoryId { get; set; }

        public string SuggestedCategoryName { get; set; } = string.Empty;

        public DateTime TransactionDate { get; set; }

        public string NormalizedNote { get; set; } = string.Empty;

        public decimal AiConfidence { get; set; }

        public List<string> MatchedKeywords { get; set; } = [];
    }
}
