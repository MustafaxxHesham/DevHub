using DevHub.AuthorizationPolicies;
using DevHub.DTOS.CourseChapters;
using DevHub.Responses;
using DevHub.Services.CoursesService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers;

[ApiController]
[Route("api/v1/courses/chapters")]
public class CoursesChaptersController(ICoursesService _coursesService) : ControllerBase
{

    [HttpPut("{chapterId}")]
    [Authorize(Policy = AuthPolicies.CourseOwnerPolicy)]
    public async Task<ActionResult> EditChapter(EditChapterRequest request)
    {
        var result = await _coursesService.EditChapterAsync(request);

        if (!result.IsSuccess)
        {

        }

        return NoContent();
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.CourseOwnerPolicy)]
    public async Task<ActionResult<CourseChapterResponse>> CreateChapter(CreateChapterRequest request)
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
        return CreatedAtAction(nameof(GetChapter), new { result.Value.Id }, result.Value);
    }

    [HttpGet("{chapterId}")]
    public async Task<ActionResult<CourseChapterResponse>> GetChapter(string chapterId)
    {
        if (chapterId.Length <= 1)
        {
            return BadRequest();
        }
        var result = await _coursesService.GetChapterByIdAsync(chapterId);

        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{chapterId}")]
    [Authorize(Policy = AuthPolicies.CourseOwnerPolicy)]
    public async Task<ActionResult> DeleteChapter(string courseId, string chapterId)
    {
        var webRootPath = HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>()
                             .WebRootPath;

        var user = HttpContext.User;

        var isUserCourseInstructor = await _coursesService.IsUserCourseInstructorAsync(user, chapterId);

        if (!isUserCourseInstructor)
        {
            return Forbid();
        }

        var result = await _coursesService.DeleteChapterAsync(courseId, chapterId, webRootPath);

        if (!result.IsSuccess)
        {
            if (result.Error.Contains("not found"))
            {
                return NotFound();
            }
        }

        return NoContent();
    }

    [HttpGet("{chaptedId}/{videourl}")]
    public Task<ActionResult> GetChapterWithVideo(string courseId, string chapterId)
    {
        throw new NotImplementedException();
    }

    [HttpPatch("chapters/reorder-chapter")]
    public async Task<ActionResult> ReorderChapter(ReOrderChapterRequest request)
    {
        var result = await _coursesService.ReOrderChapterAsync(request);

        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return NoContent();
    }

}
