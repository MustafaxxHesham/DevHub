namespace DevHub.DTOS.Courses;
public record class CreateCourseRequest(
    string Title,
    string UserId,
    string Description,
    IFormFile CourseImageFile,
    double Price,
    bool IsPublished,
    string CategoryId
);
