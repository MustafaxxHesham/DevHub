using DevHub.ActionFilters;
using DevHub.AuthorizationPolicies;
using DevHub.DTOS.CourseVideos;
using DevHub.Services.CoursesService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers;

[ApiController]
[Route("api/v1/courses")]
public class CourseVideosController(ICoursesService _coursesService) : ControllerBase
{
    [HttpPost("{courseId}/chapters/{chapterId}/upload-video")]
    [VideoFileValidator]
    public async Task<ActionResult> Post([FromBody] ChapterVideoRequest request,
        [FromRoute] string chapterId, [FromRoute] string courseId)
    {
        var result = await _coursesService.UploadChapterVideo(request, chapterId, courseId);
        throw new NotImplementedException();
    }

    [HttpDelete("{courseId}/chapters/{chapterId}/videos/{videoId}")]
    [Authorize(Policy = AuthPolicies.CourseOwnerPolicy)]
    public async Task<ActionResult> Delete(string videoId, string chapterId, string courseId)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{courseId}/chapters/{chapterId}/videos/{videoId}")]
    public async Task<ActionResult> Put(string videoId, 
        IFormFile newVideo,
        string chapterId,
        string courseId)
    {
        throw new NotImplementedException();
    }

    [HttpPatch("{courseId}/chapters/{chapterId}/videos/{videoId}")]
    public async Task<ActionResult> Patch()
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{courseId}/chapters/{chapterId}/videos")]
    public async Task<ActionResult> ClearVideos()
    {
        throw new NotImplementedException();
    }

    [HttpPost("{courseId}/chapters/{chapterId}/videos")]
    public async Task<ActionResult> UploadVideosOneTime()
    {
        throw new NotImplementedException();
    }
}
