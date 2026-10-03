namespace DevHub.Utilities;
public class KeyPagedRequest<T>(T Key, int PageSize, int PageNumber)
{
    public T Key { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public static KeyPagedRequest<T> PagedRequestCreate(T id, int pageSize, int pageNumber)
    {
        return new(id, pageSize, pageNumber);
    }
}
