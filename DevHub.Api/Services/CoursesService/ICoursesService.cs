using DevHub.Domain.Result;
using DevHub.DTOS.Commons;
using DevHub.DTOS.CourseChapters;
using DevHub.DTOS.Courses;
using DevHub.Utilities;
namespace DevHub.Services.CoursesService;
public interface ICoursesService
{
    Task<Result<CourseResponse>> GetCourseByIdAsync(string courseId);
    Task<PagedResponse<CourseResponse>> GetCoursesByCategoryAsync();
    Task<PagedResponse<CourseResponse>> GetCoursesByInstructorAsync();
    Task<PagedResponse<CourseResponse>> GetCoursesFreeAsync();
    Task<PagedResponse<CourseResponse>> GetCoursesProAsync();
    Task<PagedResponse<CourseChapterResponse>> GetChaptersAsync(CourseChapterRequest request);
    Task<PagedResponse<CourseResponse>> SearchCoursesAsync(PagedSearchRequest request);
}

