namespace DevHub.DTOS.Comments;
public record GetCommentResponse
{
    public int CommentId { get; set; }
    public string Content { get; set; }
    public string FullUserName { get; set; }
    public string ImageProfileUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public int RepliesCount { get; set; }
}