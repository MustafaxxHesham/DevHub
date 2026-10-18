using DevHub.Domain.Result;
using DevHub.DTOS.Feedbacks;
using DevHub.Utilities;

namespace DevHub.Services.FeedbackService;
public interface IFeedbacksService
{
    Task<SimpleResult<int>> CreateFeedback(CreateFeedbackRequest request);
    Task<SimpleResult<bool>> EditFeedback(EditFeedbackRequest request);
    Task<Result<PagedResponse<FeedbackResponse>>> GetFeedbackForCourse(KeyPagedRequest<string> request);
    Task<Result<PagedResponse<FeedbackResponse>>> GetFeedbackByRate(string courseId, int rate);
    Task<SimpleResult<bool>> RemoveFeedback(string feedbackId);
    Task<Result<Dictionary<string, int>>> GetFeedbackByCount(string courseId);
}