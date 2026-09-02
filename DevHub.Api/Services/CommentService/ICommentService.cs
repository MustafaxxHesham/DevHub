using DevHub.Domain.Models;
using DevHub.Domain.Result;
using DevHub.DTOS.Comments;
namespace DevHub.Services.CommentService;
public interface ICommentService
{
    Task<Result<IEnumerable<GetCommentResponse>>> GetTopCommentsAsync(int postId, int commentsCount = 7, int count = 0);
    Task<Result<Comment>> AddReplyToCommentAsync(SubmitCommentRequest request);
    Task<Result<Comment>> AddCommentAsync(SubmitCommentRequest request);
    Task<SimpleResult<bool>> DeleteCommentAsync(int commentId);
}