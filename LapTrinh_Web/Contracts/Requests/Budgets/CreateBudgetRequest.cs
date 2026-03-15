namespace LapTrinh_Web.Contracts.Requests.Budgets;

public sealed class CreateBudgetRequest
{
    public Guid CategoryId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal LimitAmount { get; set; }
}