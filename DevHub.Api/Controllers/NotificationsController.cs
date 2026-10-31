using DevHub.Services.NotificationService;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers;

[ApiController]
[Route("api/v1/notificatons")]
public class NotificationsController(INotificationsService _notificationsService) : ControllerBase
{
    [HttpGet("{userId}")]
    public async Task<ActionResult> GetUserNotification(string userId, CancellationToken cancellationToken)
    {
        return Ok(await _notificationsService.GetNotificationsAsync(userId, cancellationToken));
    }

    [HttpDelete("{notificationId}")]
    public async Task<ActionResult> DeleteNotification(string notificationId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("by-user")]
    public async Task<ActionResult> DeleteNotificationsForUser(string userId)
    {
        throw new NotImplementedException();
    }

    [HttpPatch("{notificationId}")]
    public async Task<ActionResult> ViewNotification(string notificationId)
    {
        throw new NotImplementedException();
    }


    [HttpGet("{userId}/not-viewed")]
    public async Task<ActionResult> GetCount(string userId, CancellationToken cancellationToken)
    {
        var result = await _notificationsService.GetNotViewedCountAsync(userId, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }


    [HttpPost]
    public async Task<ActionResult> NotifyAllUsers()
    {
        throw new NotImplementedException();
    }

}