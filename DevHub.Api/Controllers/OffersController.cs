using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers;

[ApiController]
[Route("api/v1/offers")]
public class OffersController : ControllerBase
{
    [HttpGet("{offerId}")]
    public ActionResult GetOfferById(string offerId)
    {
        throw new NotImplementedException();
    }

    [HttpPost("plan")]
    public ActionResult PostSubscriptionPlanOffer(object response)
    {
        throw new NotImplementedException();
    }

    [HttpPost("course")]
    public ActionResult PostCourseOffer(object response)
    {
        throw new NotImplementedException();
    }

    [HttpPatch]
    public ActionResult Patch(object response)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{offerId}")]
    public ActionResult Delete(string offerId)
    {
        throw new NotImplementedException();
    }
}
