namespace DevHub.DTOS.Comments;
public record class SubmitCommentRequest(string Content, 
    string UserId, 
    string PostId, 
    string? ParentCommentId = null);