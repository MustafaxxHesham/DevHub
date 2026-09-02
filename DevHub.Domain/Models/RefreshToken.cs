namespace DevHub.Domain.Models;

public class RefreshToken
{
    public int UserId { get; set; }
    public string Token { get; set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiredOn;
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokeOn { get; set; }
    public DateTime ExpiredOn { get; set; }
    public User User { get; set; }
    public bool IsActive { get; set; }
}