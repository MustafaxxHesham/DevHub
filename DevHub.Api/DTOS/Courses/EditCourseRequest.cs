namespace DevHub.DTOS.Courses;
public record class EditCourseRequest(
    string CourseId,
    string UserId,
    string? Title,
    string? Description,
    IFormFile? CourseImageFile,
    double? Price,
    bool? IsPublished,
    string? CategoryId
);