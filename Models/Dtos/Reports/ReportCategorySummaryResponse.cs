namespace SmartSpendAI.Models.Dtos.Reports
{
    public class ReportCategorySummaryResponse
    {
        public string CategoryName { get; set; } = string.Empty;

        public string Color { get; set; } = "#48d1a0";

        public decimal Amount { get; set; }

        public decimal Percentage { get; set; }
    }
}
