using DevHub.Domain.Result;
using DevHub.DTOS.CourseChapters;
using DevHub.DTOS.Courses;
using DevHub.DTOS.CourseVideos;
using DevHub.Utilities;
using System.Security.Claims;
namespace DevHub.Services.CoursesService;
public interface ICoursesService
{
    Task<bool> IsUserCourseInstructorAsync(ClaimsPrincipal user, string courseId);
    Task<Result<CourseResponse>> CreateCourseAsync(CreateCourseRequest request, string webRootPath);
    Task<SimpleResult<bool>> EditCourseAsync(EditCourseRequest request, string webRootPath);
    Task<SimpleResult<bool>> DeleteCourseAsync(string courseId, string webRootPath);
    Task<SimpleResult<int>> EnrollCourseAsync(EnrollCourseRequest request);
    Task<Result<PagedResponse<CourseResponse>>> GetWishListCoursesForUser(KeyPagedRequest<string> request);
    Task<Result<PagedResponse<CourseResponse>>> GetCoursesByCategoryAsync(KeyPagedRequest<string> request);
    Task<Result<PagedResponse<CourseResponse>>> GetCoursesByInstructorAsync(KeyPagedRequest<string> request);
    Task<PagedResponse<CourseResponse>> SearchCoursesAsync(KeyPagedRequest<string> request);

    Task<Result<CourseChapterResponse>> CreateChapterAsync(CreateChapterRequest request);
    Task<Result<CourseChapterResponse>> GetChapterByIdAsync(string chapterId);
    Task<SimpleResult<bool>> DeleteChapterAsync(string courseId, string chapterId, string webRootPath);
    Task<SimpleResult<bool>> EditChapterAsync(EditChapterRequest request);
    Task<SimpleResult<int>> UploadChapterVideo(ChapterVideoRequest request, string chapterId, string courseId);
    Task<SimpleResult<bool>> ReOrderChapterAsync(ReOrderChapterRequest request);
    Task<Result<CourseResponse>> GetCourseByIdAsync(string courseId);
    Task<Result<IEnumerable<CourseChapterResponse>>> GetChaptersAsync(string courseId);
    Task<Result<PagedResponse<EnrolledCourseResponse>>> GetEnrolledCoursesAsync(KeyPagedRequest<string> request);
}