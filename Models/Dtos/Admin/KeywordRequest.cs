using System.ComponentModel.DataAnnotations;

namespace SmartSpendAI.Models.Dtos.Admin
{
    public class KeywordRequest
    {
        [Required]
        [MaxLength(100)]
        public string Word { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        [Range(1, 20)]
        public int Weight { get; set; } = 5;

        public bool IsActive { get; set; } = true;
    }
}
