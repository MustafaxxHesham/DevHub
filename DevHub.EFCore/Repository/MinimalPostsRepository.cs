using DevHub.Domain.Enums;
using DevHub.Domain.LogicContract.RepositoryContract;
using DevHub.Domain.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Data.Layer.EFCore.Repository;

public class MinimalPostsRepository(AppDbContext _context) : BaseRepository<MinimalPost, int>(_context), IMinimalPostsRepository
{
    public async Task<IEnumerable<MinimalPost>> GetMinimalPostsByCategoryAsync(int categoryId, int pageNumber, int pageSize)
    {
        var pageNumberSqlParameter = new SqlParameter("pageNumber", pageNumber);
        var pageSizeSqlParameter = new SqlParameter("pageSize", pageSize);
        var categoryIdSqlParameter = new SqlParameter("categoryId", categoryId);
        var result = await _context.MinimalPosts.FromSql($"EXEC sp_GetMinimalPost {pageNumberSqlParameter}, {pageSizeSqlParameter}, {categoryIdSqlParameter}").ToListAsync();
        return result;
    }
    public async Task<IEnumerable<MinimalPost>> GetMinimalPostsPagedAsync(int pageNumber, int pageSize)
    {
        var pageNumberSqlParameter = new SqlParameter("pageNumber", pageNumber);
        var pageSizeSqlParameter = new SqlParameter("pageSize", pageSize);
        var result = await _context.MinimalPosts.FromSql($"EXEC sp_GetMinimalPost {pageNumber}, {pageSize}").ToListAsync();
        return result;
    }
    public async Task<IEnumerable<MinimalPost>> GetMinimalPostsOrderByViews(int pageNumber, int pageSize, Sorting sorting)
    {
        var pageNumberSqlParameter = new SqlParameter("pageNumber", pageNumber);
        var pageSizeSqlParameter = new SqlParameter("pageSize", pageSize);
        var result = await _context.MinimalPosts.FromSql($"EXEC sp_GetMinimalPostByViews {pageNumber}, {pageSize}, {sorting.ToString()}").ToListAsync();
        return result;
    }
    public async Task<IEnumerable<MinimalPost>> GetMinimalPostsByTagAsync(int tagId, int pageNumber, int pageSize)
    {
        var pageNumberSqlParameter = new SqlParameter("pageNumber", pageNumber);
        var pageSizeSqlParameter = new SqlParameter("pageSize", pageSize);
        var tagIdSqlParameter = new SqlParameter("tagId", tagId);
        var result = await _context.MinimalPosts
            .FromSql($"EXEC sp_GetMinimalPostByTags {pageNumberSqlParameter}, {pageSizeSqlParameter}, {tagIdSqlParameter}")
            .ToListAsync();
        return result;
    }
    public async Task<IEnumerable<MinimalPost>> GetMinimalPostsWithQueryAsync(string query, int pageNumber, int pageSize)
    {
        var pageNumberSqlParameter = new SqlParameter("pageNumber", pageNumber);
        var pageSizeSqlParameter = new SqlParameter("pageSize", pageSize);
        var querySqlParameter = new SqlParameter("query", query);
        var result = await _context.MinimalPosts
            .FromSql($"EXEC sp_GetMinimalPostSearchResult {querySqlParameter}, {pageNumberSqlParameter}, {pageSizeSqlParameter}")
            .ToListAsync();
        return result;
    }
    public async Task<IEnumerable<MinimalPost>> GetMinimalPostsBookmarkedAsync(int userId, int pageNumber, int pageSize)
    {
        var pageNumberSqlParameter = new SqlParameter("pageNumber", pageNumber);
        var pageSizeSqlParameter = new SqlParameter("pageSize", pageSize);
        var userIdParameter = new SqlParameter("userId", userId);
        var result = await _context.MinimalPosts.FromSql(
            $"EXEC sp_GetMinimalPostsBookmarked {userIdParameter}, {pageNumberSqlParameter}, {pageSizeSqlParameter}")
            .ToListAsync();
        return result;
    }
    public async Task<IEnumerable<MinimalPost>> GetPostsOrderedByViews(int pageSize, int pageNumber)
    {
        var pageNumberSqlParameter = new SqlParameter("pageNumber", pageNumber);
        var pageSizeSqlParameter = new SqlParameter("pageSize", pageSize);
        var result = await _context.MinimalPosts
            .FromSql($"EXEC sp_GetMinimalPostsOrderByViews {pageNumberSqlParameter}, {pageSizeSqlParameter}")
            .ToListAsync();
        return result;
    }
}

//GET  http://127.0.0.1:8000/    IP Host of Python Server.
//GET  http://127.0.0.1:8000/postId/recommendations IP Host of Python Server.
//GET  http://127.0.0.1:8000/posts/user/{userId}
//GET  http://127.0.0.1:8000/posts/user/{userId}/interests/
//GET  http://127.0.0.1:8000/posts/{postId}/user/{userId} check as fire-forget
//POST http://127.0.0.1:8000/events         Front-End