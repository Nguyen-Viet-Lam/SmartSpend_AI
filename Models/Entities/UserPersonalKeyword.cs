using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSpendAI.Models
{
    public class UserPersonalKeyword
    {
        [Key]
        public int UserPersonalKeywordId { get; set; }

        [Required]
        [MaxLength(120)]
        public string Keyword { get; set; } = string.Empty;

        [Required]
        public int UserId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public int UsageCount { get; set; } = 1;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;
    }
}
