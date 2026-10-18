namespace DevHub.Domain.Models;

public class Offer
{
    public int Id { get; set; }
    public double DiscountValue { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public int UserId { set; get; }
    public User User { set; get; }
    public ICollection<Course> Courses { get; set; }
    public ICollection<SubscriptionPlan> SubscriptionPlans { get; set; }
}