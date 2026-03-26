using System.ComponentModel.DataAnnotations;

namespace SmartSpendAI.Models
{
    public class KeywordEntry
    {
        [Key]
        public int KeywordEntryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Word { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public int Weight { get; set; }

        public bool IsActive { get; set; }

        public Category Category { get; set; } = null!;
    }
}
