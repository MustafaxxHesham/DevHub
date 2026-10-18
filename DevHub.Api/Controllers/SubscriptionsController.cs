using DevHub.Services.SubscriptionService;
using Microsoft.AspNetCore.Mvc;
namespace DevHub.Controllers;

[ApiController]
[Route("api/v1/subscriptions")]
public class SubscriptionsController(ISubscriptionService _subscriptionService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetSubscriptions()
    {
        throw new NotImplementedException();
    }
    public record class UserSubscribeRequest(
        string UserId,
        string PlanId
    );

    [HttpPost]
    public async Task<ActionResult> Subscribe(UserSubscribeRequest request)
    {

        throw new NotImplementedException();
    }

    [HttpGet("{planId}")]
    public async Task<ActionResult> GetPlanSubscribers(string planId)
    {
        throw new NotImplementedException();
    }

    [HttpPatch("{planId}")]
    public async Task<ActionResult> EditPlan(string planId)
    {
        throw new NotImplementedException();
    }

    [HttpPost("upgrade")]
    public async Task<ActionResult> UpgradeSubscription()
    {
        throw new NotImplementedException();
    }
}