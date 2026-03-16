using System.ComponentModel.DataAnnotations;

namespace Web_Project.Models.Dtos.Finance
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

    public class WalletResponse
    {
        public int WalletId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public decimal Balance { get; set; }

        public bool IsDefault { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
