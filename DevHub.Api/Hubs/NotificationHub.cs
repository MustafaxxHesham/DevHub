using DevHub.Services.CommentService;
using DevHub.Utilities;
using Microsoft.AspNetCore.SignalR;

namespace DevHub.Hubs;

public sealed class NotificationHub(ICommentService _commentService) : Hub<INotificationComment>
{
    public override Task OnConnectedAsync()
    {
//        Clients.
//        Console.WriteLine("A New Connection Established");
        return base.OnConnectedAsync();
    }

    public async Task CommentNotification(string comment)
    {
//        await _commentService.AddCommentAsync(comment);
        await Clients.Client(Context.ConnectionId).NotifyAuthorByComment(comment);
    }
}
