using Microsoft.AspNetCore.SignalR;

namespace LapTrinh_Web.Hubs;

public sealed class BudgetAlertHub : Hub
{
    public Task JoinUserChannel(string userChannel)
        => Groups.AddToGroupAsync(Context.ConnectionId, userChannel);

    public Task LeaveUserChannel(string userChannel)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, userChannel);
}