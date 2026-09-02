using DevHub.Domain.Helpers;
using Microsoft.EntityFrameworkCore;

namespace DevHub.EFCore.EFCorePaginationHelper;

public class PagedList<T> : IPagedList<T>
{
    public List<T> ValueList { get; private set; }
    public int CurrentPage { get; private set; }
    public int PageSize { get; private set; }
    public int TotalPages { get; private set; }
    public int TotalCount { get; private set; }

    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public PagedList(List<T> values, int pageSize, int pageNumber, int count)
    {
        CurrentPage = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        ValueList = values;
    }
    public async static Task<PagedList<T>> CreateAsync(IQueryable<T> query, int pageSize, int pageNumber)
    {
        var totalCount = await query.CountAsync();


        var values = await query.Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
        return new(values, pageSize, pageNumber, totalCount);
    }
}
