using DevHub.Domain.Result;
using DevHub.DTOS.CourseChapters;
using DevHub.DTOS.Courses;
using DevHub.DTOS.CourseVideos;
using DevHub.Utilities;
namespace DevHub.Services.CoursesService;
public interface ICoursesService
{
    Task<SimpleResult<int>> CreateChapterAsync(CreateChapterRequest request);
    Task<SimpleResult<int>> UploadChapterVideo(ChapterVideoRequest request, string chapterId, string courseId);
    Task<SimpleResult<bool>> EditChapterAsync(EditChapterRequest request);
    Task<SimpleResult<bool>> DeleteCourseAsync(string courseId, string webRootPath);
    Task<Result<CourseResponse>> GetCourseByIdAsync(string courseId);
    Task<Result<PagedResponse<CourseResponse>>> GetCoursesByCategoryAsync(KeyPagedRequest<string> request);
    Task<Result<PagedResponse<CourseResponse>>> GetCoursesByInstructorAsync(KeyPagedRequest<string> request);
    Task<PagedResponse<CourseResponse>> SearchCoursesAsync(KeyPagedRequest<string> request);
    Task<PagedResponse<CourseChapterResponse>> GetChaptersAsync(CourseChapterRequest request);
}