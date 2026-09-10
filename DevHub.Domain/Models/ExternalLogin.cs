namespace DevHub.Domain.Models;

public class ExternalLogin
{
    public int UserId { get; set; }
    public User User { get; set; }
    public string LoginKey { get; set; }
    public string Provider { get; set; }
}
