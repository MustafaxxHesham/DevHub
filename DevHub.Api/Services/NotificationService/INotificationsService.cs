using DevHub.Domain.Result;
using DevHub.Utilities;

namespace DevHub.Services.NotificationService;

public interface INotificationsService
{
    Task<PagedResponse<NotificationResponse>> GetNotificationsAsync(string userId, CancellationToken cancellationToken = default);
    Task<SimpleResult<int>> GetNotViewedCountAsync(string userId, CancellationToken cancellationToken = default);
}

public record class NotificationResponse();