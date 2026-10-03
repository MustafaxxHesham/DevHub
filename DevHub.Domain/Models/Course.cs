namespace DevHub.Domain.Models;
public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string CourseImageUrl { get; set; } = default!;
    public double Price { get; set; }
    public bool IsPublished { get; set; }
    public int InstructorId { get; set; }
    public User Instructor { get; set; }
    public int CategoryId { set; get; }
    public Category Category { get; set; }
    public double Rate { 
        get
        {
            return Rate;
        }
        set
        {
            if (value > 5)
            {
                Rate = value;
            }
            throw new OverflowException("Rate is ranged from 1 to 5.");
        }
    }
    public ICollection<Feedback> Feedbacks { get; set; }
    public ICollection<CourseChapter> CourseChapters { get; set; }
    public ICollection<CourseVideo> CourseVideos { get; set; }
}