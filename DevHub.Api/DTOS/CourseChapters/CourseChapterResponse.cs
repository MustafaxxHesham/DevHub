using DevHub.DTOS.CourseVideos;

namespace DevHub.DTOS.CourseChapters;
public record class CourseChapterResponse(
    string Id,
    string Title,
    string Description,
    int ChapterOrder,
    List<CourseVideoResponse> CourseVideoDetails
);