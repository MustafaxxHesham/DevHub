namespace DevHub.DTOS.Courses;

public record class EnrolledCourseResponse(
    string CourseId,
    string Title,
    string Description,
    string CategoryName,
    string CategoryId,
    string CourseImageUrl,
    int ProgressPercentage
);
