using DevHub.Utilities;

namespace DevHub.Services.NotificationService;

public interface INotificationsService
{
    Task<PagedResponse<NotificationResponse>> GetNotificationsAsync(string userId, CancellationToken cancellationToken = default);

}

public record class NotificationResponse();