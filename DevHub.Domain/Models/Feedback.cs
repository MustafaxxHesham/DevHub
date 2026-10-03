namespace DevHub.Domain.Models;
public class Feedback
{
    public int Id { get; set; }
    public string Content { get; set; }
    public short Rate { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; }
}