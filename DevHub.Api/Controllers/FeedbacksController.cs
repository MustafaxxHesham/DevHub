using DevHub.ActionFilters;
using DevHub.AuthorizationPolicies;
using DevHub.DTOS.Feedbacks;
using DevHub.Services.FeedbackService;
using DevHub.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers;

[ApiController]
[Route("api/v1/courses/{courseId?}/feedbacks")]
public class FeedbacksController(IFeedbacksService _feedbacksService) : ControllerBase
{
    [HttpGet("{feedbackId}")]
    public async Task<ActionResult> Get(string feedbackId)
    {
        var feedbackResult = await _feedbacksService.GetFeedbackByIdAsync(feedbackId);

        if (!feedbackResult.IsSuccess)
        {
            return NotFound(feedbackResult.Error);
        }

        return Ok(feedbackResult.Value);
    }

    [HttpPatch("{feedbackId}")]
    public async Task<ActionResult> Patch(EditFeedbackRequest request)
    {
        var result = await _feedbacksService.EditFeedback(request);

        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return NoContent();
        throw new NotImplementedException();
    }

    [HttpDelete("{feedbackId}")]
    [Authorize]
    public async Task<ActionResult> Delete(string feedbackId)
    {
        var result = await _feedbacksService.RemoveFeedback(feedbackId);

        if (!result.IsSuccess)
        {
            if (result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(result.Error);
            }
            return BadRequest(result.Error);
        }

        return NoContent();
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.AddFeedbackPolicy)]
    public async Task<ActionResult> Post(CreateFeedbackRequest request)
    {
        var result = await _feedbacksService.CreateFeedback(request);

        if (!result.IsSuccess)
        {
            if (result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(result.Error);
            }
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
    
    [HttpGet("rate-percentage")]
    public async Task<ActionResult> GetRateByCount(string courseId)
    {
        var result = await _feedbacksService.GetFeedbackByCount(courseId);

        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("{rate}")]
    public async Task<ActionResult> GetByRate(string courseId, int rate)
    {
        if (rate < 0 || rate > 5)
        {
            return BadRequest("Rate must be between 1 & 5 inclusively");
        }

        var result = await _feedbacksService.GetFeedbackByRate(courseId, rate);

        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        return Ok(result.Value);
    }

    [HttpGet("list")]
    [PaginationValidator]
    public async Task<ActionResult> GetCourseFeedbacks(KeyPagedRequest<string> request)
    {
        var pagedResult = await _feedbacksService.GetFeedbackForCourse(request);

        return Ok(pagedResult);
    }
}

public record class RateCountResponse(int TotalCount, RateCount rateCount);
public record class RateCount(char Rate, int Count);