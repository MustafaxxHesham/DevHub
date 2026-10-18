namespace DevHub.Domain.Models;

public class UserCourseTrack
{
    public int Id { get; set; }
    public int UserCourseId { get; set; }
    public UserCourse UserCourse { get; set; }
    public int ChapterId { get; set; }
    public CourseChapter Chapter { get; set; }
    public string VideoId { get; set; }
    public CourseVideo Video { get; set; }
    public DateTime WatchedAt { get; set; } = DateTime.UtcNow;
}