using DevHub.Domain.Enums;
using DevHub.Domain.Models;
namespace DevHub.Services.SubscriptionService;
public interface ISubscriptionService
{
    Task Subscribe(string userId, SubscriptionPlans Plan);
    Task<IEnumerable<HashedSubscriptionPlanResponse>> GetSubscriptionsAsync();
}