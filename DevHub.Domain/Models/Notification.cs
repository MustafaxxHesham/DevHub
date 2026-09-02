using DevHub.Domain.Enums;

namespace DevHub.Domain.Models;

public class Notification
{
    public int Id { get; set; }
    public NotificationType NotificationType { get; set; }
}
