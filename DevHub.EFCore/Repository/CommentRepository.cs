using Data.Layer.EFCore;
using Data.Layer.EFCore.Repository;
using DevHub.Domain.LogicContract;
using DevHub.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Platform.EFCore.Repository;

public class CommentRepository : BaseRepository<Comment, int>, ICommentRepository
{
    private readonly AppDbContext _context;
    public CommentRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<int> CountAsync(int commentId)
    {
         return await _context.Comments.CountAsync(c => c.ParentCommentId == commentId);
    }

    public IQueryable<Comment> GetTopCommentsAsync(int id, int commentsCount, int skipCount)
    {
        IQueryable<Comment> query = _context.Comments.Where(c => c.PostId == id)
                                                     .Include(c => c.User)
                                                     .Skip(skipCount)
                                                     .Take(commentsCount)
                                                     .OrderBy(x => x.Id)
                                                     .AsQueryable();
        return query;
    }
}
