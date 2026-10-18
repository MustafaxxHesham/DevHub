namespace DevHub.DTOS.Feedbacks;
public record class CreateFeedbackRequest(
    string CourseId,
    string UserId,
    int Rate,
    string Content);
