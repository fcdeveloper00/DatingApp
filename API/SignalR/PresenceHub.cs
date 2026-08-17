using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

[Authorize]
public class PresenceHub(PresenceTracker presenceTracker) : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (Context.User is null)
        {
            throw new HubException("Cannot get current user claim.");
        }

       bool isOnline=  await presenceTracker.UserConnected(Context.User.GetUsernameClaim(), Context.ConnectionId);

        if(isOnline)
            await Clients.Others.SendAsync("UserIsOnline", Context.User?.GetUsernameClaim());

        var currentOnlineUsers = await presenceTracker.GetOnlineUsers();
        await Clients.Caller.SendAsync("GetOnlineUsers", currentOnlineUsers);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.User is null) throw new HubException("Cannot get current user claim");

        var isOffline = await presenceTracker.UserDisconnected(Context.User.GetUsernameClaim(), Context.ConnectionId);

        if(isOffline)
            await Clients.Others.SendAsync("UserIsOffline", Context.User?.GetUsernameClaim());


        await base.OnDisconnectedAsync(exception);

    }
}
