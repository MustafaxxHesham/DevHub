using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Models;
using DevHub.Domain.Result;
using DevHub.DTOS.Comments;
using DevHub.EFCore.ErrorTypes;
using DevHub.Responses;
using DevHub.Utilities;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Services.CommentService;

public class CommentService(IDataStore _dataStore,
                            ProtectionHandler _protectionHandler,
                            ILogger<CommentService> _logger) : ICommentService
{

    public async Task<Result<Comment>> AddCommentAsync(SubmitCommentRequest request)
    {
        int realPostId = _protectionHandler.GetRealPostId(request.PostId);

        if (realPostId == -1)
        {
            return Result<Comment>.Failure(ResponseMessages.POST_NOT_FOUND);
        }

        int realUserId = _protectionHandler.GetRealUserId(request.UserId);

        if (realUserId == -1)
        {
            return Result<Comment>.Failure(ResponseMessages.USER_NOT_FOUND);
        }

        var comment = new Comment
        {
            Content = request.Content,
            PostId = realPostId,
            UserId = realUserId,
        };

        await _dataStore.Comments.AddAsync(comment);

        await _dataStore.CompleteAsync();
        
        return Result<Comment>.Success(comment);
    }
    public async Task<Result<Comment>> AddReplyToCommentAsync(SubmitCommentRequest request)
    {
        var realParentCommentId = _protectionHandler.GetRealCommentId(request.ParentCommentId!);

        if (realParentCommentId == -1 || !await _dataStore.Comments.IsExistAsync(realParentCommentId))
        {
            return Result<Comment>.Failure(ResponseMessages.COMMENT_NOT_FOUND);
        }

        var parentComment = await _dataStore.Comments.GetByIdAsync(realParentCommentId);

        if (parentComment == null) {
            return Result<Comment>.Failure(ResponseMessages.COMMENT_NOT_FOUND);
        }

        int realPostId = _protectionHandler.GetRealPostId(request.PostId);

        if (realPostId == -1 || !await _dataStore.Posts.IsExistAsync(realPostId))
        {
            return Result<Comment>.Failure(ResponseMessages.POST_NOT_FOUND);
        }


        int realUserId = _protectionHandler.GetRealUserId(request.UserId);

        if (realUserId == -1 || !await _dataStore.Users.IsExistAsync(realUserId))
        {
            return Result<Comment>.Failure(ResponseMessages.USER_NOT_FOUND);
        }

        var comment = new Comment
        {
            Content = request.Content,
            ParentCommentId = realParentCommentId,
            PostId = realPostId,
            UserId = realUserId,
            WrittenAt = DateTime.UtcNow
        };

        await _dataStore.Comments.AddAsync(comment);

        if (await _dataStore.CompleteAsync() > 0)
            return Result<Comment>.Success(comment);

        _logger.LogWarning("Error Happened While Saving Comment.");
        return Result<Comment>.Failure(DbErrors.InsertingError.ToString());
    }
    public async Task<SimpleResult<bool>> DeleteCommentAsync(int commentId)
    {
        var comment = await _dataStore.Comments.GetByCriteriaFirstAsync(c => c.Id == commentId);
        
        if (comment is null)
        {
            return SimpleResult<bool>.Failure(ResponseMessages.COMMENT_NOT_FOUND);
        }
        
        _dataStore.Comments.RemoveItem(comment);
        
        await _dataStore.CompleteAsync();

        return SimpleResult<bool>.Success(true);
    }
    public async Task<SimpleResult<bool>> EditCommentAsync(string newContent, int commentId)
    {
        var oldComment = await _dataStore.Comments.GetByIdAsync(commentId);
        
        if (oldComment is null)
        {
            return SimpleResult<bool>.Failure(ResponseMessages.COMMENT_NOT_FOUND);
        }

        oldComment.Content = newContent;

        oldComment.IsEdited = true;

        _dataStore.Comments.UpdateItem(oldComment);

        await _dataStore.CompleteAsync();

        return SimpleResult<bool>.Success(true);
    }
    public async Task<SimpleResult<int>> GetCommentsCount(string postId)
    {
        var realPostId = _protectionHandler.GetRealPostId(postId);

        if (realPostId == -1 || !await _dataStore.Posts.IsExistAsync(realPostId))
        {
            return SimpleResult<int>.Failure(ResponseMessages.POST_NOT_FOUND);
        }

        var count = await _dataStore.Comments.GetCountWithCriteriaAsync(x => x.PostId == realPostId);
        
        return SimpleResult<int>.Success(count);
    }
    public async Task<Result<IEnumerable<GetCommentResponse>>> GetTopCommentsAsync(KeyPagedRequest<int> request)
    {
        //To be Revisioned!!!
        var comments = await  _dataStore.Comments.GetTopCommentsAsync(request.Key, request.PageSize, 0)
            .GroupBy(c => c.ParentCommentId, (k, v) => new
            {
                Key = k.GetValueOrDefault(0),
                Comments = v.ToList()
            })
            .SelectMany(x => x.Comments, (x,y) => new GetCommentResponse
            {
                CommentId = y.Id,
                Content = y.Content,
                FullUserName = $"{y.User.FirstName} {y.User.LastName}",
                ImageProfileUrl = y.User.ProfileImageUrl,
                RepliesCount = x.Comments.Count
            })
            .ToListAsync();

        return Result<IEnumerable<GetCommentResponse>>.Success(comments);
    }
}
