namespace DevHub.Domain.Models;

public class WishList
{
    public int UserId { get; set; }
    public User User { get; set; }
    public Course Course { get; set; }
    public int CourseId { get; set; }
}