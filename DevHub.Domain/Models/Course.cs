namespace DevHub.Domain.Models;
public class Course
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string CourseImageUrl { get; set; }
    public int InstructorId { get; set; }
    public User Instructor { get; set; }
    public ICollection<CourseChapter> CourseChapters { get; set; }
    public ICollection<CourseVideo> CourseVideos { get; set; }
}
