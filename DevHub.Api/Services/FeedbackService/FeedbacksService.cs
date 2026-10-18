using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Enums;
using DevHub.Domain.Models;
using DevHub.Domain.Result;
using DevHub.DTOS.Feedbacks;
using DevHub.Responses;
using DevHub.Utilities;

namespace DevHub.Services.FeedbackService;

public class FeedbacksService(ProtectionHandler _protectionHandler, IDataStore _dataStore) : IFeedbacksService
{
    public async Task<SimpleResult<int>> CreateFeedback(CreateFeedbackRequest request)
    {
        if (string.IsNullOrEmpty(request.Content))
        {
            return SimpleResult<int>.Failure("Content cannot be empty");
        }

        int courseId = _protectionHandler.GetRealCourseId(request.CourseId.Trim());
        
        int userId = _protectionHandler.GetRealUserId(request.UserId.Trim());
        
        short rate = (short)request.Rate;

        if (rate < 1 || rate > 5)
        {
            return SimpleResult<int>.Failure("Rate must be between 1 and 5");
        }

        if (courseId == -1 || userId == -1)
        {
            return SimpleResult<int>.Failure("ERROR"); // TO BE Handled
        }

        if (!await _dataStore.Courses.IsExistAsync(courseId))
        {
            return SimpleResult<int>.Failure(ResponseMessages.COURSE_NOT_FOUND);
        }


        if (!await _dataStore.Users.IsExistAsync(userId))
        {
            return SimpleResult<int>.Failure(ResponseMessages.USER_NOT_FOUND);
        }

        var feedback = new Feedback
        {
            CourseId = courseId,
            UserId = userId,
            Rate = rate,
            Content = request.Content.Trim(),
            
        };

        await _dataStore.Feedbacks.AddAsync(feedback);

        await _dataStore.CompleteAsync();

        return SimpleResult<int>.Success(feedback.Id);
    }

    public async Task<SimpleResult<bool>> EditFeedback(EditFeedbackRequest request)
    {
        var protectedFeedbackId = _protectionHandler.GetRealFeedbackId(request.FeedbackId.Trim());

        if (protectedFeedbackId == -1)
        {
            return SimpleResult<bool>.Failure(ResponseMessages.EntityNotFound(nameof(Feedback)));
        }

        var feedback = await _dataStore.Feedbacks.GetByIdAsync(protectedFeedbackId);

        if (feedback is null)
        {
            return SimpleResult<bool>.Failure(ResponseMessages.EntityNotFound(nameof(Feedback)));
        }

        if (request.Rate.HasValue)
        {
            feedback.Rate = (short)request.Rate.Value;
        }

        if (!string.IsNullOrEmpty(request.Content.Trim()))
        {
            feedback.Content = request.Content.Trim();
        }

        _dataStore.Feedbacks.UpdateItem(feedback);
        
        await _dataStore.CompleteAsync();

        return SimpleResult<bool>.Success(true);
    }

    public async Task<Result<Dictionary<string, int>>> GetFeedbackByCount(string courseId)
    {
        var realCourseId = _protectionHandler.GetRealCourseId(courseId);

        if (realCourseId == -1 || await _dataStore.Courses.IsExistAsync(realCourseId))
        {
            return Result<Dictionary<string, int>>.Failure(ResponseMessages.EntityNotFound(nameof(Course)));
        }

        return Result<Dictionary<string, int>>.Success(await _dataStore.Feedbacks.GetRateCountedAsync(realCourseId));
    }

    public async Task<Result<PagedResponse<FeedbackResponse>>> GetFeedbackByRate(string courseId, int rate)
    {
        var realCourseId = _protectionHandler.GetRealCourseId(courseId);

        if (realCourseId == -1 || await _dataStore.Courses.IsExistAsync(realCourseId))
        {
            return Result<PagedResponse<FeedbackResponse>>.Failure(ResponseMessages.EntityNotFound(nameof(Course)));
        }

        var feedbackList = await _dataStore.Feedbacks.GetPagedFeedbacksByRateAsync(rate, 6, 1, realCourseId, 0);

        var feedbackResponses = MapFeedbacksToResponses(feedbackList.ValueList);

        var response = PagedResponse<FeedbackResponse>.Create(feedbackResponses, 6, 1, 0);

        return Result<PagedResponse<FeedbackResponse>>.Success(response);
    }

    public async Task<Result<PagedResponse<FeedbackResponse>>> GetFeedbackForCourse(KeyPagedRequest<string> request)
    {
        var courseId = _protectionHandler.GetRealCourseId(request.Key.Trim());

        if (courseId == -1 || !await _dataStore.Courses.IsExistAsync(courseId))
        {
            return Result<PagedResponse<FeedbackResponse>>.Failure(ResponseMessages.COURSE_NOT_FOUND);
        }

        var pagedList = await _dataStore.Feedbacks.GetPagedFeedbacksAsync(
            request.PageSize,
            request.PageNumber,
            courseId,
            request.LastKeyId,
            Sorting.Ascending);

        var feedbackResponseList = MapFeedbacksToResponses(pagedList.ValueList);


        var pagedResult = PagedResponse<FeedbackResponse>.Create(feedbackResponseList, pagedList.PageSize, pagedList.CurrentPage, pagedList.CurrentPage);

        return Result<PagedResponse<FeedbackResponse>>.Success(pagedResult);
    }

    public async Task<SimpleResult<bool>> RemoveFeedback(string feedbackId)
    {
        var realFeedbackId = _protectionHandler.GetRealFeedbackId(feedbackId.Trim());

        if (realFeedbackId == -1)
        {
            return SimpleResult<bool>.Failure(ResponseMessages.EntityNotFound(nameof(Feedback)));
        }

        var feedback = await _dataStore.Feedbacks.GetByIdAsync(realFeedbackId);

        if (feedback is null)
        {
            return SimpleResult<bool>.Failure(ResponseMessages.EntityNotFound(nameof(Feedback)));
        }

        _dataStore.Feedbacks.RemoveItem(feedback);

        await _dataStore.CompleteAsync();

        return SimpleResult<bool>.Success(true);
    }

    private List<FeedbackResponse> MapFeedbacksToResponses(IEnumerable<Feedback> feedbacks)
    {
       return feedbacks.Select(f => new FeedbackResponse(
            FeedbackId:_protectionHandler.GetProtectedFeedbackId(f.Id),
            UserId:_protectionHandler.GetProtectedUserId(f.UserId),
            ReviewerFullName: f.User.FirstName + " " + f.User.LastName,
            ReviewerImageProfile: f.User.ProfileImageUrl,
            Rate:f.Rate,
            Content:f.Content,
            CreatedAt:f.CreatedAt
       )).ToList();
    }
}