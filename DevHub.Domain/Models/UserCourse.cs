namespace DevHub.Domain.Models;
public class UserCourse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; }
    public DateTime EnrolledAt { get; set; }
    public double WatchedPercentage { get; set; } = 0.00;
    public ICollection<UserCourseTrack> UserCourseTrackers { get; set; }
}