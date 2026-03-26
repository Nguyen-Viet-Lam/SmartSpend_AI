namespace SmartSpendAI.Models.Dtos.Dashboard
{
    public class CategoryBreakdownDto
    {
        public string CategoryName { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public decimal Amount { get; set; }
    }
}
