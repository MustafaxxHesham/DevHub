namespace DevHub.DTOS.Feedbacks;

public record class EditFeedbackRequest(
    string FeedbackId,
    int? Rate,
    string? Content
);
