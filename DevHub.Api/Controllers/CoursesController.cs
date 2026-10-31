using DevHub.ActionFilters;
using DevHub.AuthorizationPolicies;
using DevHub.DTOS.Courses;
using DevHub.Services.CoursesService;
using DevHub.Utilities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.ComponentModel.DataAnnotations;

namespace DevHub.Controllers;

[ApiController]
[Route("api/v1/courses")]
public class CoursesController(ICoursesService _coursesService) : ControllerBase
{
    [HttpGet("{courseId}")]
    public async Task<ActionResult<CourseResponse>> Get(string courseId)
    {
        if (string.IsNullOrEmpty(courseId))
        {
            return BadRequest("course id not sent!");
        }

        var result = await _coursesService.GetCourseByIdAsync(courseId);

        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [Idempotency]
    [Authorize(Policy = AuthPolicies.AddCoursePolicy)]
    public async Task<ActionResult> Post(CreateCourseRequest request, [FromServices]IValidator<CreateCourseRequest> validator)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        var webRootPath = HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().WebRootPath;

        var result = await _coursesService.CreateCourseAsync(request, webRootPath);

        if (!result.IsSuccess)
        {
            BadRequest();
        }

        return CreatedAtAction(nameof(Get), new { result.Value.CourseId }, result.Value);
    }

    [HttpDelete("{courseId}")]
    [Authorize(Policy = AuthPolicies.CourseOwnerPolicy)]
    public async Task<ActionResult> Delete(string courseId)
    {
        // Deletion Requires Refund for non-completed users.
        var webRootPath = HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().WebRootPath;

        var deletionResult = await _coursesService.DeleteCourseAsync(courseId, webRootPath);

        if (!deletionResult.IsSuccess)
        {
            return StatusCode(500); // Not Final Result
        }

        return NoContent();
    }

    [HttpPut("{courseId}")]
    [Authorize(Policy = AuthPolicies.CourseOwnerPolicy)]
    public async Task<ActionResult> Put(EditCourseRequest request)
    {
        var webRootPath = HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().WebRootPath;
        var result = await _coursesService.EditCourseAsync(request, webRootPath);
        if (!result.IsSuccess)
        {
            if (result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound();
            }
            // to be revisioned!
        }
        return NoContent();        
    }

    [HttpPatch("{courseId}/add-offer/{offerPercentage}")]
    [Authorize(Policy = AuthPolicies.CourseOwnerPolicy)]
    public async Task<ActionResult> AttachOffer(string courseId, double offerPercentage)
    {
        throw new NotImplementedException();
    }

    [HttpPost("enroll-course")]
    [Idempotency]
    public async Task<ActionResult> EnrollCourse(EnrollCourseRequest request, CancellationToken cancellationToken)
    {
        var idempotencyKey = HttpContext.Request.Headers["X-Idempotency-Key"]!;

        var result = await _coursesService.EnrollCourseAsync(request);

        if (!result.IsSuccess)
        {
            if (result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(result.Error);
            }

            return Conflict();
        }

        await HttpContext.RequestServices.GetRequiredService<IDistributedCache>()
                         .CompleteIdempotentItemAsync(idempotencyKey!);

        return Ok();
    }

    [HttpGet("query/{query}")]
    [PaginationValidator]
    public async Task<ActionResult> SearchCourses(KeyPagedRequest<string> request)
    {
        return Ok(await _coursesService.SearchCoursesAsync(request));
    }
    
    // To Be Revisioned As Temporary Solution
    public record class FilteredPagedRequest(string Query, string Plan);
    [HttpGet("{query}/filter/{course-type}")]
    [PaginationValidator]
    public async Task<ActionResult> FilterCourses(FilteredPagedRequest request)
    {
        if (string.IsNullOrEmpty(request.Query))
        {
            return BadRequest("Search value is empty!");
        }

        int pageSize = int.Parse(HttpContext.Request.Headers["X-PageSize"]!);

        int pageNumber = int.Parse(HttpContext.Request.Headers["X-PageNumber"]!);

        var pagedRequest = KeyPagedRequest<string>.PagedRequestCreate(request.Query, pageSize, pageNumber);

        var result = await _coursesService.SearchCoursesAsync(pagedRequest);

        return Ok(result);
    }

    [HttpGet("{courseId}/chapters/{chaptedId}")]
    public async Task<ActionResult> GetByChapters([Required]string courseId)
    {
        var chaptersList = await _coursesService.GetChaptersAsync(courseId);

        throw new NotImplementedException();
    }

    [HttpGet("wishlisted-courses/users/{userId}")]
    public async Task<ActionResult> GetWishlistedAsync(KeyPagedRequest<string> request)
    {
        var result = await _coursesService.GetWishListCoursesForUser(request);
        
        if (result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(result.Error);
        }
     
        return Ok(result.Value);
    }

    [HttpGet("enrolled-courses/users/{userId}")]
    [PaginationValidator]
    public async Task<ActionResult<PagedResponse<CourseResponse>>> GetEnrolledCourses(KeyPagedRequest<string> request)
    {
        var result = await _coursesService.GetEnrolledCoursesAsync(request);

        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("instructor/{instructorId}")]
    [PaginationValidator]
    public async Task<ActionResult> GetInstructorCourses(KeyPagedRequest<string> request)
    {
        var result = await _coursesService.GetCoursesByInstructorAsync(request);
        
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

}