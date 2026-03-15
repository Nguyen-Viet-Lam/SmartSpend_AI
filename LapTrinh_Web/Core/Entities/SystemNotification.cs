using LapTrinh_Web.Core.Enums;

namespace LapTrinh_Web.Core.Entities;

public class SystemNotification : BaseEntity
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public BudgetAlertLevel AlertLevel { get; set; } = BudgetAlertLevel.Normal;
    public bool IsRead { get; set; }
}