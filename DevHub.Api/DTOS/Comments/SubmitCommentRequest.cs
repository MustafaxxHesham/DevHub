namespace DevHub.DTOS.Comments;

public record SubmitCommentRequest(
    string Content,
    int UserId,
    int PostId,
    int? ParentCommentId = null) {}