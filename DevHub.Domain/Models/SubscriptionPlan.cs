namespace DevHub.Domain.Models;
public class SubscriptionPlan
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string PlanMessage { get; set; }
    public double Price { get; set; }
    public ICollection<SubscriptionFeature> SubscriptionFeatures { get; set; }
}