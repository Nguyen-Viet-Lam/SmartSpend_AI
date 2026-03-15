using LapTrinh_Web.Core.Enums;
using LapTrinh_Web.Services.Interfaces;

namespace LapTrinh_Web.Services.Implementations;

public sealed class NotificationService : INotificationService
{
    public Task PushBudgetAlertAsync(Guid userId, string title, string message, BudgetAlertLevel alertLevel, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task SendSecurityLoginAlertAsync(Guid userId, string ipAddress, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}