namespace DevHub.Utilities;
public class PagedRequest(string CrtieriaId, int PageSize, int PageNumber)
{
    public string CriteriaId { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public static PagedRequest PagedRequestFactory(string id, int pageSize, int pageNumber)
    {
        return new(id, pageSize, pageNumber);
    }
}
