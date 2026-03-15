namespace LapTrinh_Web.Core.Entities;

public class BudgetPlan : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal LimitAmount { get; set; }
}