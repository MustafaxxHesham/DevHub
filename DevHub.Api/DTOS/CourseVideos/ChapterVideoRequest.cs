namespace DevHub.DTOS.CourseVideos;
public record class ChapterVideoRequest(IFormFile CourseVideo,
    int Order,
    bool IsPreview,
    bool ExtractTitleFromVideoName,
    string? Title = null);