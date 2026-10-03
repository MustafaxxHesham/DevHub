using DevHub.ActionFilters;
using DevHub.DTOS.CourseChapters;
using DevHub.DTOS.Courses;
using DevHub.DTOS.CourseVideos;
using DevHub.Responses;
using DevHub.Services.CoursesService;
using DevHub.Utilities;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPatch("")]
    public async Task<ActionResult> EditChapter(string chapterId, string title)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{courseId}")]
    public async Task<ActionResult> Delete(string courseId)
    {
        var webRootPath = HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().WebRootPath;

        var deletionResult = await _coursesService.DeleteCourseAsync(courseId, webRootPath);

        if (!deletionResult.IsSuccess)
        {
            return StatusCode(500); // Not Final Result
        }

        return NoContent();
    }

    [HttpPut("{courseId}")]
    public Task<ActionResult> Put(string courseId)
    {
        throw new NotImplementedException();
    }
    
    [HttpPost("{courseId}/chapter")]
    public async Task<ActionResult> AddChapter(CreateChapterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState.Values);
        }
        var result = await _coursesService.CreateChapterAsync(request);

        if (!result.IsSuccess)
        {
            if (result.Error.Equals(ResponseMessages.COURSE_NOT_FOUND))
            {
                return NotFound(result.Error);
            }
            return BadRequest(result.Error);
        }
        return CreatedAtAction("", "");// To be checked....
    }

    [HttpGet("{courseId}/chpater/{chaptedId}")]
    public Task<ActionResult> GetChapterWithVideo(string courseId, string chapterId)
    {
        throw new NotImplementedException();
    }

    [HttpGet("minimal-course/{courseId}")]
    public Task<ActionResult<IEnumerable<CourseResponse>>> GetMinimalCourse()
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

        var pagedRequest = KeyPagedRequest<string>.PagedRequestCreate(query, pageSize, pageNumber);

        var pagedResponse = await _coursesService.SearchCoursesAsync(pagedRequest);

        return Ok(pagedResponse);
    }
    
    // To Be Revisioned As Temporary Solution
    public record class FilteredPagedRequest(string Query, string Plan);
    [HttpGet("minimal-course/{query}/filter/{course-type}")]
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
    public async Task<ActionResult> GetByChapters(CourseChapterRequest request)
    {
        var chaptersList = await _coursesService.GetChaptersAsync(request);
        throw new NotImplementedException();
    }

    [HttpPost("{courseId}/chapters/{chapterId}/upload-video")]
    [VideoFileValidator]
    public async Task<ActionResult> UploadVideo([FromBody]ChapterVideoRequest request, [FromRoute]string chapterId, [FromRoute]string courseId)
    {
        var result = await _coursesService.UploadChapterVideo(request, chapterId, courseId);
        throw new NotImplementedException();
    }
}