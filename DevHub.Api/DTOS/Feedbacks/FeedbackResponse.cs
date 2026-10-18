namespace DevHub.DTOS.Feedbacks;

public record class FeedbackResponse(
    string FeedbackId,
    string ReviewerFullName,
    string ReviewerImageProfile,
    string UserId,
    int Rate,
    string Content,
    DateTime CreatedAt
);