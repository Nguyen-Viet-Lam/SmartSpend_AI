namespace LapTrinh_Web.Contracts.Responses.Dashboard;

public sealed class DashboardSummaryResponse
{
    public decimal TotalBalance { get; set; }
    public decimal TotalExpense7Days { get; set; }
    public decimal TotalIncome7Days { get; set; }
}