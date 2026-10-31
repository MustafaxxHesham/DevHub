using DevHub.Services.NotificationService;
using DevHub.Utilities;
using Microsoft.AspNetCore.SignalR;

namespace DevHub.Hubs;

public sealed class NotificationHub(ProtectionHandler _protectionHandler) : Hub<INotificationActions>
{
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    public async Task CommentNotification(string comment)
    {
        await Clients.Client(Context.ConnectionId).NotifyAuthorByComment(comment);
    }

    public async Task NotifyUser(NotificationResponse response)
    {

        //await Clients.User().;
    }
}
