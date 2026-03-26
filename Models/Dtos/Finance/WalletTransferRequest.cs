using System.ComponentModel.DataAnnotations;

namespace SmartSpendAI.Models.Dtos.Finance
{
    public class WalletTransferRequest
    {
        [Required]
        public int FromWalletId { get; set; }

        [Required]
        public int ToWalletId { get; set; }

        [Range(0.01, 999999999)]
        public decimal Amount { get; set; }

        [MaxLength(280)]
        public string Note { get; set; } = string.Empty;

        public DateTime? TransferDate { get; set; }
    }
}
