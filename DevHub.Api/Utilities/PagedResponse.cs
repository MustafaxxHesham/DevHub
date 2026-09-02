namespace DevHub.Utilities;
public class PagedResponse<Response>
{
    public List<Response> Values { get; set; }
    public int CurrentPage { get; private set; }
    public int PageSize { get; private set; }
    public int TotalPages { get; private set; }
    public int TotalCount { get; private set; }

    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
    public PagedResponse(List<Response> values, int pageSize, int pageNumber, int totalPages)
    {
        Values = values;
        CurrentPage = pageNumber;
        PageSize = pageSize;
        TotalPages = totalPages;

    }

    public static PagedResponse<Response> Create(List<Response> values, int pageSize, int currentPage, int totalPages)
    {
        return new(values, pageSize, currentPage, totalPages);
    }
}
