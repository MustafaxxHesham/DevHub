namespace DevHub.Domain.Helpers;
public interface IPagedList<T>
{
    public List<T> ValueList { get; }
    public int CurrentPage { get; }
    public int PageSize { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
}
