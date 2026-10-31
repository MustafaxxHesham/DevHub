using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Enums;
using DevHub.Domain.Models;
using DevHub.Domain.Result;
using DevHub.Hubs;
using DevHub.Responses;
using DevHub.Utilities;
namespace DevHub.Services.NotificationService;
public class NotificationsService(IDataStore _dataStore,
    ProtectionHandler _protectionHandler,
    NotificationHub _hub) : INotificationsService
{
    public async Task<PagedResponse<NotificationResponse>> GetNotificationsAsync(string userId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }


    public async Task<string> CreateNotification(CreateNotificationRequest request)
    {
        var notification = GetNotification(request);

        throw new NotImplementedException();
    }

    public async Task<SimpleResult<int>> GetNotViewedCountAsync(string userId, CancellationToken cancellationToken = default)
    {
        var realUserId = _protectionHandler.GetRealUserId(userId);

        if (realUserId == -1)
        {
            return SimpleResult<int>.Failure(ResponseMessages.USER_NOT_FOUND);
        }

        var notificationsCount = await _dataStore.Notifications.GetCountWithCriteriaAsync(n => n.ReceiverId == realUserId && !n.IsViewed);

        return SimpleResult<int>.Success(notificationsCount);
    }

    private Notification GetNotification(CreateNotificationRequest request)
    {
        return new Notification
        {
            IsViewed = false,
            NotificationType = request.NotificationType,
            CreatedAt = DateTime.UtcNow,
            Message = request.Message,
            ReceiverId = _protectionHandler.GetRealUserId(request.UserId)
        };
    }
}


public record class CreateNotificationRequest(
    string Message,
    NotificationType NotificationType,
    string UserId
);