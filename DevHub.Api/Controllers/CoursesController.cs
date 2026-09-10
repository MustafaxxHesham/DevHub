using DevHub.ActionFilters;
using DevHub.DTOS.Commons;
using DevHub.DTOS.CourseChapters;
using DevHub.DTOS.Courses;
using DevHub.Services.CoursesService;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DevHub.Controllers;

[ApiController]
[Route("api/v1/courses")]
public class CoursesController(ICoursesService _coursesService) : ControllerBase
{
    [HttpGet("{courseId}")]
    public async Task<ActionResult<CourseResponse>> Get(string courseId)
    {
        var course = await _coursesService.GetCourseByIdAsync(courseId);

        if (!course.IsSuccess)
        {
            return NotFound(course.Error);
        }

        return Ok(course.Value);
    }

    [HttpDelete("{courseId}")]
    public Task<ActionResult> Delete(string courseId)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{courseId}")]
    public Task<ActionResult> Put(string courseId)
    {
        throw new NotImplementedException();
    }

    [HttpPatch("{courseId}")]
    public Task<ActionResult> Patch(string courseId)
    {
        throw new NotImplementedException();
    }

    [HttpPost("{courseId}/chapter")]
    public Task<ActionResult> AddChapter(string courseId)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{courseId}/chpater/{chaptedId}")]
    public Task<ActionResult> GetChapterWithVideo(string courseId, string chapterId)
    {
        throw new NotImplementedException();
    }

    [HttpGet("minimal-course/{courseId}")]
    public Task<ActionResult> GetMinimalCourse()
    {
        throw new NotImplementedException();
    }

    [HttpGet("minimal-course/{query}")]
    [PaginationValidator]
    public async Task<ActionResult> SearchCourses(string query)
    {
        if (string.IsNullOrEmpty(query))
        {
            return BadRequest("Search value is empty!");
        }

        int pageSize = int.Parse(HttpContext.Request.Headers["X-PageSize"]!);
        int pageNumber = int.Parse(HttpContext.Request.Headers["X-PageNumber"]!);

        var pagedRequest = new PagedSearchRequest(query, pageSize, pageNumber);

        var result = await _coursesService.SearchCoursesAsync(pagedRequest);

        return Ok(result);
    }
    
    // To Be Revisioned As Temporary Solution
    public record class FilteredPagedRequest(string Query, string Plan);
    [HttpGet("minimal-course/{query}/filter/{course-type}")]
    public async Task<ActionResult> FilterCourses(FilteredPagedRequest request)
    {
        if (string.IsNullOrEmpty(request.Query))
        {
            return BadRequest("Search value is empty!");
        }

        int pageSize = int.Parse(HttpContext.Request.Headers["X-PageSize"]!);

        int pageNumber = int.Parse(HttpContext.Request.Headers["X-PageNumber"]!);

        var pagedRequest = new PagedSearchRequest(request.Query, pageSize, pageNumber);

        var result = await _coursesService.SearchCoursesAsync(pagedRequest);

        return Ok(result);
    }

    [HttpGet("{courseId}/chapters/{chaptedId}")]
    public async Task<ActionResult> GetByChapters(CourseChapterRequest request)
    {
        var chaptersList = await _coursesService.GetChaptersAsync(request);
        throw new NotImplementedException();
    }

    [HttpPost("{courseId}/chapters/{chapterId}/upload-video")]
    [VideoFileValidator]
    public async Task<ActionResult> UploadVideo(IFormFile courseVideo)
    {
        throw new NotImplementedException();
    }
}