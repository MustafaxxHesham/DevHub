using DevHub.ActionFilters;
using DevHub.Domain.Models;
using DevHub.DTOS.Report;
using DevHub.EFCore.ErrorTypes;
using DevHub.Responses;
using DevHub.Services.ReportingService;
using DevHub.Utilities;
using Microsoft.AspNetCore.Authorization;
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

        var pagedRequestResult = KeyPagedRequest<string>.PagedRequestCreate(userId, pageSize, pageNumber);

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
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> GetReports()
    {
        int pageSize = int.Parse(HttpContext.Request.Headers["X-PageSize"]!);
        int pageNumber = int.Parse(HttpContext.Request.Headers["X-PageNumber"]!);

        var result = await _reportService.GetReportsAsync(pageSize, pageNumber);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

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

        var keyPagedRequest = KeyPagedRequest<int>.PagedRequestCreate(weekNum, pageSize, pageNumber);

        var result = await _reportService.GetWeeklyReportsAsync(keyPagedRequest);

        if (!result.IsSuccess)
        {
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

    [HttpGet("posts-weekly/{weekCount}")]
    [PaginationValidator]
    public async Task<ActionResult> GetReportsByPostsWeekly(int weekCount)
    {
        if (weekCount <= 0)
        {
            return BadRequest();
        }
        int pageSize = int.Parse(HttpContext.Request.Headers["X-PageSize"]!);
        
        int pageNumber = int.Parse(HttpContext.Request.Headers["X-PageNumber"]!);

        var keyPagedRequest = KeyPagedRequest<int>.PagedRequestCreate(weekCount, pageSize, pageNumber);

        var result = await _reportService.GetWeeklyReportsAsync(keyPagedRequest);

        return Ok(result);
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
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> AnswerReport(ReportAnswerRequest request)
    {
        var result = await _reportService.AnswerReportAsync(request);
        if (!result.IsSuccess)
        {
            if (result.Error.Contains("Not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(result.Error);
            }
            if (result.Error.Equals(ResponseMessages.ACTION_ALREADY_DONE))
            {
                return Conflict("You already answered this report.");
            }
            return BadRequest(result.Error);
        }

        // _nofityUser(.....)
        return Ok(result.Value);
    }

    [HttpGet("answer/{reportId}")]
    public async Task<ActionResult> GetAnsweredReport(string reportId)
    {
        var result = await _reportService.GetAnswerReportAsync(reportId);
        if (!result.IsSuccess)
        {
            if (result.Error.Equals(ResponseMessages.REPORT_ANSWER_NOT_FOUND))
            {
                return NotFound(ResponseMessages.REPORT_ANSWER_NOT_FOUND);
            }
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }

    [HttpGet("answer/{reportId}/users/{userId}")]
    public async Task<ActionResult> GetAnsweredReportsForUser(string userId)
    {
        throw new NotImplementedException();
    }
}

