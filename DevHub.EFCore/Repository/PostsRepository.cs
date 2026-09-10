using DevHub.Domain.LogicContract.RepositoryContract;
using DevHub.Domain.Models;
using DevHub.Domain.Result;
using Microsoft.EntityFrameworkCore;
using System.Text;

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
        
        var query = _context.Posts.Include(p => p.Tags)
                                  .Include(p => p.Author)
                                  .Include(p => p.Comments)
                                  .AsQueryable();

        return query;
    }

    public Task<IQueryable<Post>> GetPostsByCategoryAsync(string category)
    {
        int x = 5;
        HashSet<int> postIds = new([54, 55, 2]);
        //  Elasticsearch
        _context.Posts.Where(p => EF.Functions.FreeText(p, p.Content));
        throw new NotImplementedException();
    }

    public async Task<Dictionary<string, int>> GetPostsCountByCategoryAsync()
    {
        var result = await _context.Categories.GroupJoin(_context.Posts,
            c => c.Id,
            p => p.CategoryId,
            (c, p) => new { CategoryName = c.Name, PostsCount = p.Count() })
            .ToDictionaryAsync(x => x.CategoryName, x => x.PostsCount);

        return result;
    }

    public async Task<Dictionary<string, int>> GetPostsCountByTagAsync()
    {
        var result = await _context.Tags
            .Select(tag => new
            {
                TagName = tag.Name,
                PostsCount = tag.Posts.Count()
            })
            .ToDictionaryAsync(x => x.TagName, x => x.PostsCount);
        return result;
    }

    public Task UpdatePostAsync(Post post)
    {
        throw new NotImplementedException();
    }


    public async Task C()
    {
        StringBuilder stringBuilder = new StringBuilder();
        var x = new object[] { new { k = 4, v = 54 }, new { k = 4, v = 54 } };
        var queryBeginning = "UPDATE POSTS SET ViewsCount = CASE Id ";
        //Ensure here that is numeric value & positive
        foreach (var item in x)
        {
            stringBuilder.Append($"WHEN {item} THEN {item}");
        }
        stringBuilder.Append("ELSE ViewsCount");
        stringBuilder.Append("END");
        stringBuilder.Append(@"WHERE Id IN (
        
        )");
        var finalScript = stringBuilder.ToString();
        _context.Posts.FromSql($"{finalScript}");
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
