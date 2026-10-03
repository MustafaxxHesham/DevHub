namespace DevHub.Domain.Models;
public class PostsViews
{
    public int Id { get; set; }
    public bool IsUserFollowingAuthor { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int PostId { get; set; }
    public Post Post { get; set; }
}