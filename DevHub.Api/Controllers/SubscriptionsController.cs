using Microsoft.AspNetCore.Mvc;
namespace DevHub.Controllers;

[ApiController]
[Route("api/v1/subscriptions")]
public class SubscriptionsController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetSubscriptions()
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<ActionResult> Subscribe()
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
}