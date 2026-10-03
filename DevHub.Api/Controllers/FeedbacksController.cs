using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers;

[ApiController]
[Route("api/v1/feedback")]
public class FeedbacksController : ControllerBase
{
    [HttpGet("{feedbackId}")]
    public ActionResult Get(string feedbackId)
    {
        throw new NotImplementedException();
    }

    [HttpPatch("{feedbackId}")]
    public ActionResult Patch(string feedbackId)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{feedbackId}")]
    public ActionResult Delete(string feedbackId)
    {
        throw new NotImplementedException();
    }
    public record class CreateFeedbackRequest(string Details, string UserId, string CourseId, char Rate);

    [HttpGet("courseId")]
    public ActionResult Post()
    {
        throw new NotImplementedException();
    }
    [HttpGet("rate-percentage")]
    public async Task<ActionResult> GetRateByCount()
    {

        // {Rate : 1, Count: 14}
        // {Rate : 2, Count: 14}
        // {Rate : 3, Count: 14}
        // {Rate : 4, Count: 14}
        // {Rate : 5, Count: 14}
        throw new NotImplementedException();
    }


    [HttpGet("/{rate}")]
    public async Task<ActionResult> GetByRate(int rate)
    {
        throw new NotImplementedException();
    }

}

public record class RateCountResponse(int TotalCount, RateCount rateCount);

public record class RateCount(char Rate, int Count);