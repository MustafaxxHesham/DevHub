using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Enums;
using DevHub.Domain.Models;
using DevHub.Utilities;

namespace DevHub.Services.SubscriptionService;

public class SubscriptionService(IDataStore _dataStore, ProtectionHandler _protectionHandler) : ISubscriptionService
{
    public async Task<IEnumerable<HashedSubscriptionPlanResponse>> GetSubscriptionsAsync()
    {
        var subscriptions = await _dataStore.SubscriptionPlans.GetAllAsync(["SubscriptionFeatures"]);

        var hashedSubscriptions = GetHashedSubscriptionPlans(subscriptions);

        return hashedSubscriptions;
    }

    public Task Subscribe(string userId, SubscriptionPlans Plan)
    {
        throw new NotImplementedException();
    }

    private List<HashedSubscriptionPlanResponse> GetHashedSubscriptionPlans(IEnumerable<SubscriptionPlan> subscriptionPlans)
    {
        return subscriptionPlans.Select(plan => new HashedSubscriptionPlanResponse(
            Id: _protectionHandler.GetProtectedSubscriptionId(plan.Id),
            Name: plan.Name,
            Description: plan.PlanMessage,
            Price: plan.Price,
            features: plan.SubscriptionFeatures.ToList()
        )).ToList();
    }
}

public record class HashedSubscriptionPlanResponse(string Id, string Name, string Description, double Price, List<SubscriptionFeature> features);