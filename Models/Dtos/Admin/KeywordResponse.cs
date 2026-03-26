namespace SmartSpendAI.Models.Dtos.Admin
{
    public class KeywordResponse
    {
        public int KeywordId { get; set; }

        public string Word { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public int Weight { get; set; }

        public bool IsActive { get; set; }
    }
}
