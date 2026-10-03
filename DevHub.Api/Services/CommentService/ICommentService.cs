using DevHub.Domain.Models;
using DevHub.Domain.Result;
using DevHub.DTOS.Comments;
using DevHub.Utilities;
namespace DevHub.Services.CommentService;
public interface ICommentService
{
    Task<SimpleResult<int>> GetCommentsCount(string postId);
    Task<Result<IEnumerable<GetCommentResponse>>> GetTopCommentsAsync(KeyPagedRequest<int> request);
    Task<Result<Comment>> AddReplyToCommentAsync(SubmitCommentRequest request);
    Task<Result<Comment>> AddCommentAsync(SubmitCommentRequest request);
    Task<SimpleResult<bool>> DeleteCommentAsync(int commentId);
    Task<SimpleResult<bool>> EditCommentAsync(string newComment, int commentId);
}