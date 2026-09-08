using DevHub.ActionFilters;
using DevHub.Domain.Models;
using DevHub.DTOS.Report;
using DevHub.EFCore.ErrorTypes;
using DevHub.Services.ReportingService;
using DevHub.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers;

[ApiController]
[Route("api/v1/reports")]
//[Authorize]
public class ReportsController(IReportService _reportService) : ControllerBase
{
    [HttpGet("users/{userId}")]
    [PaginationValidator]
    public async Task<ActionResult<PagedResponse<ReportResponse>>> GetReportsByUser(string userId)
    {
        if (string.IsNullOrEmpty(userId) || userId.Equals("0"))
        {
            return BadRequest();
        }
        int pageSize = int.Parse(HttpContext.Request.Headers["X-PageSize"]!);

        int pageNumber = int.Parse(HttpContext.Request.Headers["X-PageNumber"]!);

        var pagedRequestResult = PagedRequest.PagedRequestFactory(userId, pageSize, pageNumber);

        var result = await _reportService.GetReportsByUserAsync(pagedRequestResult);

        if (!result.IsSuccess)
        {
            if (!result.Error.Equals(DbErrors.NotFoundError.ToString()))
            {
                return Ok(result.Value);
            }
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }

    [HttpGet("{reportId}")]
    public async Task<ActionResult<ReportResponse>> Get(string reportId)
    {
        var result = await _reportService.GetReportAsync(reportId);
        if (!result.IsSuccess)
        {
            if (result.Error.Equals(DbErrors.NotFoundError))
                return NotFound();
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult<Report>> Post(SaveReportRequest request)
    {
        var result = await _reportService.SaveReportAsync(request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        //Error Check
        return CreatedAtAction(nameof(Get), new { reportId = 5 }, result.Value);
    }

    [HttpDelete("{reportId}")]
    public async Task<ActionResult> Delete(string reportId)
    {
        var result = await _reportService.DeleteReportAsync(reportId);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }

    [HttpGet]
    [PaginationValidator]
    public async Task<ActionResult> GetReports()
    {
        int pageSize = int.Parse(HttpContext.Request.Headers["X-PageSize"]!);
        int pageNumber = int.Parse(HttpContext.Request.Headers["X-PageNumber"]!);

        var result = await _reportService.GetReportsAsync(pageSize, pageNumber);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("monthly/{month}")]
    [PaginationValidator]
    public async Task<ActionResult> GetReportsMonthly(int month)
    {
        if (month > 12 || month < 0)
        {
            return BadRequest("Month number must be any number from 1 to 12.");
        }

        int pageSize = int.Parse(HttpContext.Request.Headers["X-PageSize"]!);

        int pageNumber = int.Parse(HttpContext.Request.Headers["X-PageNumber"]!);

        var result = await _reportService.GetMonthlyReportsAsync(new PagedReportMonthlyRequest(month, pageSize, pageNumber, null));

        if (!result.IsSuccess)
        {
            if (result.Error.Equals(DbErrors.NotFoundError.ToString()))
            {
                return NotFound();
            }
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("weekly/{week-number}")]
    public async Task<ActionResult> GetReportsWeekly([FromRoute(Name = "week-number")] int weekNum)
    {
        if (weekNum >= 0)
        {
            return BadRequest("Week number must be greater than or equal 0.");
        }

        int pageSize = int.Parse(HttpContext.Request.Headers["X-PageSize"]!);

        int pageNumber = int.Parse(HttpContext.Request.Headers["X-PageNumber"]!);

        var result = await _reportService.GetWeeklyReportsAsync(weekNum, pageSize, pageNumber);

        if (!result.IsSuccess)
        {
            if (result.Error.Equals(DbErrors.NotFoundError.ToString()))
            {
                return NotFound();
            }
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("posts")]
    public async Task<ActionResult> GetReportsByPosts()
    {
        int pageSize = int.Parse(HttpContext.Request.Headers["X-PageSize"]!);

        int pageNumber = int.Parse(HttpContext.Request.Headers["X-PageNumber"]!);

        var result = await _reportService.GetReportsForPostsAsync(pageSize, pageNumber);

        if (!result.IsSuccess)
        {
            if (result.Error.Equals(DbErrors.NotFoundError.ToString()))
            {
                return Ok(result.Value);
            }
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    // To be reviewed!
    [HttpGet("posts-monthly/{monthId}")]
    public async Task<ActionResult> GetReportsByPostsMonthly(PagedReportMonthlyRequest request)
    {
        if (request.Month > 12 || request.Month < 0)
        {
            return BadRequest($"Month must start by 1 and end to 12");
        }

        var result = await _reportService.GetMonthlyReportsAsync(request);

        if (!result.IsSuccess)
        {
            if (result.Error.Equals(DbErrors.NotFoundError.ToString()))
            {
                return Ok(result.Value);
            }
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("posts-weekly")]
    public async Task<ActionResult> GetReportsByPostsWeekly()
    {
        throw new NotImplementedException();
    }

    [HttpGet("count-not-viewed")]
    [PaginationValidator]
    public async Task<ActionResult> GetNotViewed()
    {
        int pageSize = int.Parse(HttpContext.Request.Headers["X-PageSize"]!);
        
        int pageNumber = int.Parse(HttpContext.Request.Headers["X-PageNumber"]!);

        var result = await _reportService.GetReportsByAdminViewAsync(false, pageSize, pageNumber);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPatch("viewed-by-admin")]
    public async Task<ActionResult> MarkAsViewed(string reportId)
    {
        var result = await _reportService.MarkReportAsViewedAsync(reportId);

        if (!result.IsSuccess)
        {
            if (result.Error.Equals(DbErrors.NotFoundError.ToString()))
            {
                return NotFound();
            }
            return BadRequest(result.Error);
        }

        return NoContent();
    }

    [HttpPost("{reportId}/answer")]
    public async Task<ActionResult> AnswerReport(string reportId)
    {
        throw new NotImplementedException();
    }
}