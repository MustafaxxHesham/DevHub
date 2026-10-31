using DevHub.Services.CoursesService;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers;

[ApiController]
[Route("api/v1/not-determined")]
public class CourseTrackController(ICoursesService _coursesSerivce) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetMyCoursesAsync()
    {
        throw new NotImplementedException();
    }
}