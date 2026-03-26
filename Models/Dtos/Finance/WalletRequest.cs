using System.ComponentModel.DataAnnotations;

namespace SmartSpendAI.Models.Dtos.Finance
{
    public class WalletRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(32)]
        public string Type { get; set; } = "Cash";

        [Range(0, 999999999)]
        public decimal InitialBalance { get; set; }

        public bool IsDefault { get; set; }
    }
}
