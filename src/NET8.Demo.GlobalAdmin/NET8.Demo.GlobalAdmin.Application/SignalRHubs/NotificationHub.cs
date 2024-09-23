using Microsoft.AspNetCore.SignalR;
using NET8.Demo.GlobalAdmin.Application.Contracts.IServices;

namespace NET8.Demo.GlobalAdmin.Application.SignalRHubs;

public class NotificationHub : Hub
{
    private readonly IConnectionManager _connectionManager;
    public NotificationHub(IConnectionManager connectionManager)
    {
        _connectionManager = connectionManager;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier ?? Context.ConnectionId;
        if (userId != null)
        {
            await _connectionManager.AddConnectionAsync(userId, Context.ConnectionId);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = Context.UserIdentifier ?? Context.ConnectionId;
        if (userId != null)
        {
            await _connectionManager.RemoveConnectionAsync(userId, Context.ConnectionId);
        }
        await base.OnDisconnectedAsync(exception);
    }
}
