using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers;

[ApiController]
[Route("api/v1/courses")]
public class CoursesController : ControllerBase
{
    [HttpGet("{courseId}")]
    public Task<ActionResult> Get(string courseId)
    {
        throw new NotImplementedException();
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
}
