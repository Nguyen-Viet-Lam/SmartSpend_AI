using LapTrinh_Web.Core.Enums;

namespace LapTrinh_Web.Services.Interfaces;

public interface INotificationService
{
    Task PushBudgetAlertAsync(Guid userId, string title, string message, BudgetAlertLevel alertLevel, CancellationToken cancellationToken = default);
    Task SendSecurityLoginAlertAsync(Guid userId, string ipAddress, CancellationToken cancellationToken = default);
}