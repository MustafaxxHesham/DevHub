namespace DevHub.Domain.Models;
public class SubscriptionFeature
{
    public int Id { get; set; }
    public string FeatureName { get; set; }
    public int Limit { get; set; }
    public int DurationLimitInDays { get; set; }
    public int PlanId { get; set; }
    public SubscriptionPlan Plan { get; set; }
}