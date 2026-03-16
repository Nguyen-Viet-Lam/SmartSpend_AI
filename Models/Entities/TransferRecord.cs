using System.ComponentModel.DataAnnotations;

namespace Web_Project.Models
{
    public class TransferRecord
    {
        [Key]
        public int TransferRecordId { get; set; }

        public int FromWalletId { get; set; }

        public int ToWalletId { get; set; }

        public decimal Amount { get; set; }

        [MaxLength(280)]
        public string Note { get; set; } = string.Empty;

        public DateTime TransferDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public Wallet FromWallet { get; set; } = null!;

        public Wallet ToWallet { get; set; } = null!;
    }
}
