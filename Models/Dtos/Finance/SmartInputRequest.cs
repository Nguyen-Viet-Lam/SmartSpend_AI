using System.ComponentModel.DataAnnotations;

namespace SmartSpendAI.Models.Dtos.Finance
{
    public class SmartInputRequest
    {
        [Required]
        [MaxLength(300)]
        public string Input { get; set; } = string.Empty;
    }
}
