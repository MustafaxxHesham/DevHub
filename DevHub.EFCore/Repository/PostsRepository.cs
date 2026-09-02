using DevHub.Domain.Enums;
using DevHub.Domain.LogicContract.RepositoryContract;
using DevHub.Domain.Models;
using DevHub.Domain.Result;
using Microsoft.EntityFrameworkCore;

namespace Data.Layer.EFCore.Repository;

public class PostsRepository(AppDbContext _context) : BaseRepository<Post, int>(_context), IPostRepository
{
    public Task<SimpleResult<int>> CreatePostAsync(Post post)
    {
        throw new NotImplementedException();
    }

    public async Task DeletePostAsync(Post post)
    {
        await _context.Database.BeginTransactionAsync();
        await _context.BookmarkedPosts.Where(bp => bp.PostId == post.Id).ExecuteDeleteAsync();
        _context.Posts.Remove(post);
        await _context.Database.CommitTransactionAsync();
    }

    public IQueryable<Post> GetPostDetailsAsync()
    {
        
        var query = _context.Posts
                            .Include(p => p.Tags)
                            .Include(p => p.Author)
                            .Include(p => p.Comments)
                            .AsQueryable();

        return query;
    }

    public Task<Result<IQueryable<Post>>> GetPostsByCategoryAsync(string category)
    {
        int x = 5;
        HashSet<int> postIds = new([54, 55, 2]);
        //  Elasticsearch
        _context.Posts.Where(p => EF.Functions.FreeText(p, p.Content));
        throw new NotImplementedException();
    }


    public Task<Result<IQueryable<Post>>> GetPostsOrderedByViewsCountAsync(int pageSize, int pageNumber)
    {
        throw new NotImplementedException();
    }

    public Task UpdatePostAsync(Post post)
    {
        throw new NotImplementedException();
    }



    /*        public async Task<IEnumerable<Post>> GetPostsByCategoryAsync(string category)
            {
                return await _context.Posts.Where(x => x.Category.Name == category).ToListAsync();
            }

            public async Task<IEnumerable<Post>> GetPostsBySearchAsync(string search, int pageSize, int pageNumber)
            {
                return await _context.Posts.Where(x => x.Title.Contains(search))
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }

            public async Task<IEnumerable<Post>> GetPostsBySearchAsync(string search)
            {
                return await _context.Posts.Where(x => x.Title.Contains(search)).ToListAsync();
            }

            public async Task<IEnumerable<Post>> GetPostsOrderedByDateAsync(string search, Sorting sortingDirection)
            {
                return await _context.Posts.Where(x => x.Title.Contains(search))
                    .OrderBy(x => x.CreatedAt)
                    .ToListAsync();
            }

            public async Task<IEnumerable<Post>> GetPostsOrderedByViewsCountAsync()
            {
                return await _context.Posts.OrderByDescending(x => x.ViewsCount)
                        .ToListAsync();
            }

            public async Task<IEnumerable<Post>> GetPostsOrderedByViewsCountAsync(int pageSize, int pageNumber)
            {
                return await _context.Posts.OrderByDescending(x => x.ViewsCount)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();
            }*/
}
