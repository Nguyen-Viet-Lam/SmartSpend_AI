namespace Web_Project.Models.Dtos.Dashboard
{
    public class DashboardResponse
    {
        public decimal TotalBalance { get; set; }

        public decimal TotalIncomeThisMonth { get; set; }

        public decimal TotalExpenseThisMonth { get; set; }

        public int UnreadAlerts { get; set; }

        public List<TrendPointDto> MonthlyTrend { get; set; } = [];

        public List<CategoryBreakdownDto> ExpenseBreakdown { get; set; } = [];

        public List<Web_Project.Models.Dtos.Finance.BudgetResponse> BudgetProgress { get; set; } = [];

        public List<string> Insights { get; set; } = [];

        public List<string> Forecasts { get; set; } = [];
    }

    public class TrendPointDto
    {
        public string Label { get; set; } = string.Empty;

        public decimal Income { get; set; }

        public decimal Expense { get; set; }
    }

    public class CategoryBreakdownDto
    {
        public string CategoryName { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public decimal Amount { get; set; }
    }
}
