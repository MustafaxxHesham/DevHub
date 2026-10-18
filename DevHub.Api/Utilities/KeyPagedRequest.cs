using DevHub.CustomModelBinders;
using DevHub.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Utilities;

[ModelBinder(BinderType = typeof(KeyedRequestModelBinder))]
public class KeyPagedRequest<T>(T Key,
    int PageSize, 
    int PageNumber,
    int? lastId, Sorting? sortingDirection = Sorting.Ascending)
{
    public T Key { get; set; } = Key;
    public int PageSize { get; set; } = PageSize;
    public int PageNumber { get; set; } = PageNumber;
    public Sorting? SortingDirection { set; get; } = Sorting.Ascending;
    public int? LastKeyId { set; get; } = lastId;
    public static KeyPagedRequest<T> PagedRequestCreate(T id, 
        int pageSize, 
        int pageNumber,
        int? lastId = null,
        Sorting? sortingDirection = Sorting.Ascending)
    {
        return new(id, pageSize, pageNumber, lastId, sortingDirection);
    }
}
