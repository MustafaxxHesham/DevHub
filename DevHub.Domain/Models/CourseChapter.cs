namespace DevHub.Domain.Models;
public class CourseChapter
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int ChapterNumber { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; }
    public ICollection<CourseVideo> CourseVideos { get; set; }
}
