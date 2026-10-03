using DevHub.Domain.Enums;
namespace DevHub.Domain.Models;
public class UserSubscription
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int SubscriptionPlanId { get; set; }
    public SubscriptionPlan SubscriptionPlan { get; set; }
    public bool AutoSubscribe { get; set; }
    public BillingCycle BillingCycle { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}