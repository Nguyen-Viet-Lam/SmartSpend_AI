using System.ComponentModel.DataAnnotations;

namespace SmartSpendAI.Models.Dtos.Finance
{
    public class LearnFromCorrectionRequest
    {
        [Required]
        [MaxLength(300)]
        public string Input { get; set; } = string.Empty;

        [Required]
        public int CorrectedCategoryId { get; set; }
    }
}
