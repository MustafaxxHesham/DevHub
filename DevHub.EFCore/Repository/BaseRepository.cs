using DevHub.Domain.Helpers;
using DevHub.Domain.LogicContract;
using DevHub.EFCore.EFCorePaginationHelper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
namespace Data.Layer.EFCore.Repository;

public class BaseRepository<T, K>(AppDbContext _context) : IBaseRepository<T, K> where T : class
{
    // Inserting Methods Signatures
    public async Task<T> AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        return entity;
    }
    public async Task AddBulkAsync(IEnumerable<T> entities)
    {
        await _context.Set<T>().AddRangeAsync(entities);
    }


    // Deleting Methods Signatures
    public void DeleteBulk(IEnumerable<T> entities)
    {
        _context.Set<T>().RemoveRange(entities);
    }
    public void RemoveItem(T entity)
    {
        _context.Set<T>().Remove(entity);
    }


    // Updating Methods Signatures
    public void UpdateItem(T entity)
    {
        _context.Set<T>().Update(entity);
    }


    // Retrieving Methods Signatures --> Search, Paginated 
    public async Task<IEnumerable<T>> GetAllAsync() 
    {
        return await _context.Set<T>().ToListAsync();
    }
    public async Task<IPagedList<T>> GetByCriteriaAsync(Expression<Func<T, bool>> criteria, Expression<Func<T, object>> orderByCriteria,
        [Optional] int? pageSize, [Optional] int? pageNumber, [Optional] string[]? includeProps)
    {
        var query = _context.Set<T>().AsQueryable();

        query = query.Where(criteria);

        if (includeProps.Any())
        {
            foreach (var prop in includeProps)
            {
                query = query.Include(prop);
            }
        }

        query = query.OrderBy(orderByCriteria);

        var result = await PagedList<T>.CreateAsync(query, pageSize.Value, pageNumber.Value);

        return result;
    }
    public async Task<T> GetByCriteriaFirstAsync(Expression<Func<T, bool>> criteria, [Optional] string[]? includeProps)
    {
        var query = _context.Set<T>().AsQueryable();

        query = query.Where(criteria).AsQueryable();

        if (includeProps.Any())
        {
            foreach (var prop in includeProps)
            {
                query = query.Include(prop);
            }
        }

        var item = await query.FirstOrDefaultAsync();

        return item;
    }
    public async Task<T> GetByIdAsync(K id) => (await _context.Set<T>().FindAsync(id))!;
    public async Task<IPagedList<T>> GetPaginatedAsync(int pageSize, int pageNumber, Expression<Func<T, object>> orderByCriteria, [Optional] string[]? includeProps)
    {
        var query = _context.Set<T>().AsQueryable();

        if (includeProps.Any())
        {
            foreach (var prop in includeProps)
            {
                query = query.Include(prop);
            }
        }

        query = query.OrderBy(orderByCriteria).AsQueryable();

        var result = await PagedList<T>.CreateAsync(query, pageSize, pageNumber);

        return result;
    }
    public async Task<IPagedList<T>> GetPaginatedByCriteriaAsync(int pageSize, int pageNumber, Expression<Func<T, bool>> criteria, Expression<Func<T, object>> orderByCriteria, [Optional] string[]? includeProps)
    {
        var query = _context.Set<T>().AsQueryable();

        query = query.Where(criteria);

        if (includeProps.Any())
        {
            foreach (var prop in includeProps)
            {
                query = query.Include(prop);
            }
        }

        var pagedList = await PagedList<T>.CreateAsync(query, pageSize, pageNumber);

        return pagedList;
    }



    // Counting Methods Signatures
    public async Task<int> GetCountAsync()
    {
        return await _context.Set<T>().CountAsync();
    }
    public async Task<int> GetCountWithCriteriaAsync(Expression<Func<T, bool>> criteria)
    {
        return await _context.Set<T>().Where(criteria).CountAsync();
    }
    

    // Checking Methods Signatures
    public async Task<bool> IsExistAsync(K id)
    {
        return await _context.Set<T>().FindAsync(id) is not null;
    }
    public async Task<bool> IsExistWithCriteriaAsync(Expression<Func<T, bool>> criteria)
    {
        return await _context.Set<T>().CountAsync(criteria) > 0;
    }
}
