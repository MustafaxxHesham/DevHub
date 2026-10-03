namespace DevHub.DTOS.CourseVideos;
public record class CourseVideoResponse(string VideoUrl, DateTime VideoDuration, string VideoTitle, bool IsPreview);