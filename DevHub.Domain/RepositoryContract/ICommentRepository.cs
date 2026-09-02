using DevHub.Domain.Models;
namespace DevHub.Domain.LogicContract;
public interface ICommentRepository : IBaseRepository<Comment, int>
{
    IQueryable<Comment> GetTopCommentsAsync(int id, int commentsCount, int skipCount);
    Task<int> CountAsync(int commentId);
}