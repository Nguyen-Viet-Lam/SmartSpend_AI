namespace SmartSpendAI.Models.Dtos.Dashboard
{
    public class TrendPointDto
    {
        public string Label { get; set; } = string.Empty;

        public decimal Income { get; set; }

        public decimal Expense { get; set; }
    }
}
