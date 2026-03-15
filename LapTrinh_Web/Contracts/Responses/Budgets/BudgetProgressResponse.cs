namespace LapTrinh_Web.Contracts.Responses.Budgets;

public sealed class BudgetProgressResponse
{
    public Guid BudgetId { get; set; }
    public Guid CategoryId { get; set; }
    public decimal LimitAmount { get; set; }
    public decimal UsedAmount { get; set; }
    public decimal UsagePercent { get; set; }
}