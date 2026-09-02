using DevHub.Domain.Enums;
namespace DevHub.Domain.Models;
public class SubscriptionPlan
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public DateTime IssuedAt { get; set; }
    public BillingCycle BillingCycle { get; set; }
}