using DevHub.Domain.Enums;
namespace DevHub.Domain.Models;
public class Notification
{
    public int Id { get; set; }
    public string Message { get; set; }
    public bool IsViewed { get; set; }
    public DateTime CreatedAt { get; set; }
    public NotificationType NotificationType { get; set; }
    public int ReceiverId { set; get; }
    public User Receiver { set; get; }
}