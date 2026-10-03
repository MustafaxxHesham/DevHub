using DevHub.Domain.Enums;
using DevHub.Domain.Helpers;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace DevHub.Domain.LogicContract;

public interface IBaseRepository<T, K> where T : class
{
    Task<T> GetByIdAsync(K id, [Optional] string[]? includeProps);
    Task<T> GetByCriteriaFirstAsync(Expression<Func<T, bool>> criteria, [Optional] string[]? includeProps);
    Task<IPagedList<T>> GetByCriteriaAsync(Expression<Func<T, bool>> criteria, 
                                            Expression<Func<T, object>> orderByCriteria,
                                            [Optional] int? pageSize, 
                                            [Optional] int? pageNumber, 
                                            [Optional] string[]? includeProps);
    Task<IEnumerable<T>> GetListByCriteriaAsync(Expression<Func<T, bool>> criteria, [Optional] string[]? includeProps);
    Task<IPagedList<T>> GetPaginatedAsync(int pageSize, int pageNumber, Expression<Func<T, object>> orderByCriteria, [Optional] string[]? includeProps,
        Sorting orderingDirection = Sorting.Ascending);
    Task<IPagedList<T>> GetPaginatedByCriteriaAsync(int pageSize, int pageNumber, 
        Expression<Func<T, bool>> criteria, 
        Expression<Func<T, object>> orderByCriteria, 
        [Optional] string[]? includeProps,
        Sorting orderingDirection = Sorting.Ascending);

    Task<IEnumerable<T>> GetAllAsync([Optional] string[]? includeProps);
    Task<int> GetCountAsync();
    Task<int> GetCountWithCriteriaAsync(Expression<Func<T, bool>> criteria);
    Task<bool> IsExistAsync(K id);
    Task<bool> IsExistWithCriteriaAsync(Expression<Func<T, bool>> criteria);
    Task<T> AddAsync(T entity);
    Task AddBulkAsync(IEnumerable<T> entities);
    void DeleteBulk(IEnumerable<T> entities);
    void UpdateItem(T entity);
    void RemoveItem(T entity);
}
