using Data.Layer.EFCore;
using Data.Layer.EFCore.Repository;
using DevHub.Domain.Enums;
using DevHub.Domain.Helpers;
using DevHub.Domain.Models;
using DevHub.Domain.RepositoryContract;
using DevHub.EFCore.EFCorePaginationHelper;
using Microsoft.EntityFrameworkCore;

namespace DevHub.EFCore.Repository;

public class FeedbackRepository(AppDbContext _context) : BaseRepository<Feedback, int>(_context), IFeedbackRepository
{
    public Task<Feedback> GetFeedbackById(int feedbackId)
    {
        var query = _context.Feedbacks.AsQueryable();
        
        query = query.Select(f => new Feedback
        {
            Content = f.Content,
            CreatedAt = f.CreatedAt,
            Id = f.Id,
            Rate = f.Rate,
            User = new User
            {
                FirstName = f.User.FirstName,
                LastName = f.User.LastName,
                ProfileImageUrl = f.User.ProfileImageUrl,
            }
        });

        return query.FirstOrDefaultAsync();
    }

    public async Task<IPagedList<Feedback>> GetPagedFeedbacksAsync(int pageSize, 
        int pageNumber, 
        int courseId,
        int? lastId,
        Sorting sorting = Sorting.Ascending)
    {
        var query = _context.Feedbacks
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        if (sorting == Sorting.Ascending)
        {
            query = query.Where(c => c.CourseId == courseId && lastId.HasValue ? c.Id > lastId : true)
                         .OrderBy(c => new { c.CreatedAt, c.Id });
        }
        else
        {
            query = query.Where(c => c.CourseId == courseId && lastId.HasValue ? c.Id < lastId : true)
                         .OrderByDescending(c => new { c.CreatedAt, c.Id });
        }

        query = query.Select(f => new Feedback
        {
            Content = f.Content,
            CreatedAt = f.CreatedAt,
            Id = f.Id,
            Rate = f.Rate,
            User =  new User
            {
                FirstName = f.User.FirstName,
                LastName = f.User.LastName,
                ProfileImageUrl = f.User.ProfileImageUrl,
            }
        });

        return await PagedResponse<Feedback>.CreateAsync(query, pageSize, pageNumber);
    }

    public async Task<IPagedList<Feedback>> GetPagedFeedbacksByRateAsync(int rate, 
        int pageSize,
        int pageNumber, 
        int courseId,
        int? lastId,
        Sorting sorting = Sorting.Ascending)
    {
        var query = _context.Feedbacks
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize);

        if (sorting == Sorting.Ascending)
        {
            query = query.Where(c => c.CourseId == courseId && rate == c.Rate && lastId.HasValue ? c.Id > lastId : true)
                         .OrderBy(c => c.CreatedAt)
                         .ThenBy(c => c.Id);
        }
        else
        {
            query = query.Where(c => c.CourseId == courseId && rate == c.Rate && lastId.HasValue ? c.Id < lastId : true)
                         .OrderByDescending(c => c.CreatedAt)
                         .ThenByDescending(c => c.Id);
        }

        query = query.Select(f => new Feedback
        {
            Content = f.Content,
            CreatedAt = f.CreatedAt,
            Id = f.Id,
            Rate = f.Rate,
            User = new User
            {
                FirstName = f.User.FirstName,
                LastName = f.User.LastName,
                ProfileImageUrl = f.User.ProfileImageUrl,
            }
        });

        return await PagedResponse<Feedback>.CreateAsync(query, pageSize, pageNumber);
    }

    public async Task<Dictionary<string, int>> GetRateCountedAsync(int courseId)
    {
        return await _context.Feedbacks.Where(f => f.CourseId == courseId).GroupBy(x => x.Rate)
            .ToDictionaryAsync(x => x.Key.ToString(), x => x.Count());
    }
}
