namespace DevHub.Domain.Models;

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; }
    public int PostId { get; set; }
    public DateTime WrittenAt { get; set; }
    public Post Post { get; set; }
    public int UserId { get; set; }
    public User User{ get; set; }
    public bool IsEdited { get; set; } = false;
    public int? ParentCommentId { get; set; }
}
