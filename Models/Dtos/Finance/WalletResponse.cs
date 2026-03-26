namespace SmartSpendAI.Models.Dtos.Finance
{
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
