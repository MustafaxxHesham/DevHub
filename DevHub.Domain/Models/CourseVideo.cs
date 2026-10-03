namespace DevHub.Domain.Models;

public class CourseVideo
{
    public string Id { get; set; }
    public string VideoUrl { get; set; }
    public string Title { get; set; }
    public DateTime Duration{ get; set; }
    public bool IsPreview { get; set; }
    public int CourseChapterId { get; set; }
    public CourseChapter Chapter { get; set; }
    public int? CourseId { get; set; }
    public Course Course{ get; set; }
}
