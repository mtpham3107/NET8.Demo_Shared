using Microsoft.AspNetCore.SignalR;

namespace NET8.Demo.GlobalAdmin.Application.SignalRHubs;

public class CustomUserIdProvider : DefaultUserIdProvider
{
    public override string GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst("sub")?.Value;
    }
}
