namespace DevHub.Domain.Models;
public class Wallet
{
    public Guid Id { get; set; }
    public DateTime IssuedAt { get; set; }
    public double Balance { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}