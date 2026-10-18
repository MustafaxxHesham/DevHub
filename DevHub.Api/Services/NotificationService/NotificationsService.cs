using DevHub.Utilities;
namespace DevHub.Services.NotificationService;
public class NotificationsService : INotificationsService
{
    public async Task<PagedResponse<NotificationResponse>> GetNotificationsAsync(string userId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task CreateNotification()
    {
        throw new NotImplementedException();
    }
}
