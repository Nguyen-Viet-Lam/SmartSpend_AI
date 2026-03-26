using System.ComponentModel.DataAnnotations;

namespace SmartSpendAI.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(32)]
        public string Type { get; set; } = "Expense";

        [MaxLength(48)]
        public string Icon { get; set; } = string.Empty;

        [MaxLength(16)]
        public string Color { get; set; } = string.Empty;

        public bool IsSystem { get; set; }

        public ICollection<TransactionEntry> Transactions { get; set; } = new List<TransactionEntry>();

        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();

        public ICollection<KeywordEntry> Keywords { get; set; } = new List<KeywordEntry>();

        public ICollection<UserPersonalKeyword> PersonalKeywords { get; set; } = new List<UserPersonalKeyword>();
    }
}
