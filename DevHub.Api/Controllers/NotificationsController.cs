using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers;

[ApiController]
[Route("api/v1/notificatons")]
public class NotificationsController : ControllerBase
{
    [HttpGet("{userId}")]
    public async Task<ActionResult> GetUserNotification(string userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteNotification([FromQuery(Name = "notificationId")]string notificationId)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("by-user")]
    public async Task<ActionResult> DeleteNotificationsForUser([FromQuery(Name = "userId")] string userId)
    {
        throw new NotImplementedException();
    }

    [HttpPatch("{notificationId}")]
    public async Task<ActionResult> ViewNotification([FromQuery(Name = "notificationId")] string notificationId)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{userId}/not-viewed")]
    public async Task<ActionResult> GetCount()
    {
        throw new NotImplementedException();
    }

}