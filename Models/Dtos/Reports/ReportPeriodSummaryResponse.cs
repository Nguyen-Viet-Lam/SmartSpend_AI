namespace SmartSpendAI.Models.Dtos.Reports
{
    public class ReportPeriodSummaryResponse
    {
        public string PeriodLabel { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal TotalIncome { get; set; }

        public decimal TotalExpense { get; set; }

        public decimal NetAmount { get; set; }

        public int TransactionCount { get; set; }

        public List<ReportCategorySummaryResponse> TopExpenseCategories { get; set; } = [];
    }
}
