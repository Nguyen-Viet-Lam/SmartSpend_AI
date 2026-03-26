using System.ComponentModel.DataAnnotations;

namespace SmartSpendAI.Models
{
    public class Wallet
    {
        [Key]
        public int WalletId { get; set; }

        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(32)]
        public string Type { get; set; } = "Cash";

        public decimal Balance { get; set; }

        public bool IsDefault { get; set; }

        public DateTime CreatedAt { get; set; }

        public User User { get; set; } = null!;

        public ICollection<TransactionEntry> Transactions { get; set; } = new List<TransactionEntry>();

        public ICollection<TransferRecord> IncomingTransfers { get; set; } = new List<TransferRecord>();

        public ICollection<TransferRecord> OutgoingTransfers { get; set; } = new List<TransferRecord>();
    }
}
