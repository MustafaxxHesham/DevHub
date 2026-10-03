namespace DevHub.DTOS.CourseVideos;
public record class ChapterVideoRequest(IFormFile CourseVide,
    int Order,
    bool IsPreview,
    bool ExtractTitleFromVideoName,
    string? Title = null);