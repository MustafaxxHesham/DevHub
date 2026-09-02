using DevHub.Domain.DataStoreContract;
using DevHub.Domain.LogicContract;
using DevHub.Domain.Models;
using DevHub.Domain.Result;
using DevHub.DTOS.Comments;
using DevHub.EFCore.ErrorTypes;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Services.CommentService;
public class CommentService(ICommentRepository _commentRepo, IDataStore _dataStore, ILogger<CommentService> _logger) : ICommentService
{
    public async Task<Result<Comment>> AddCommentAsync(SubmitCommentRequest request)
    {
        var comment = new Comment
        {
            Content = request.Content,
            PostId = request.PostId,
            UserId = request.UserId,
        };

        await _commentRepo.AddAsync(comment);

        if (await _dataStore.CompleteAsync() > 0)
            return Result<Comment>.Success(comment);

        return Result<Comment>.Failure(DbErrors.InsertingError.ToString());

    }
    public async Task<Result<Comment>> AddReplyToCommentAsync(SubmitCommentRequest request)
    {
        var parentComment = await _commentRepo.GetByIdAsync(request.ParentCommentId!.Value);

        if (parentComment == null) {
            return Result<Comment>.Failure(DbErrors.NotFoundError.ToString());
        }

        var comment = new Comment
        {
            Content = request.Content,
            ParentCommentId = request.ParentCommentId,
            PostId = request.PostId,
            UserId = request.UserId,
            WrittenAt = DateTime.UtcNow
        };

        await _commentRepo.AddAsync(comment);

        if (await _dataStore.CompleteAsync() > 0)
            return Result<Comment>.Success(comment);

        _logger.LogWarning("Error Happened While Saving Comment.");
        return Result<Comment>.Failure(DbErrors.InsertingError.ToString());
    }
    public async Task<SimpleResult<bool>> DeleteCommentAsync(int commentId)
    {
        var comment = await _commentRepo.GetByCriteriaFirstAsync(c => c.Id == commentId);
        if (comment is null)
        {
        }
        throw new NotImplementedException();
    }
    public async Task<Result<IEnumerable<GetCommentResponse>>> GetTopCommentsAsync(int postId, int commentsCount, int count = 0)
    {
        var comments = await  _commentRepo.GetTopCommentsAsync(postId, commentsCount, count)
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
