namespace DevHub.DTOS.Commons;
public record class PagedSearchRequest(string searchKey, int pageSize, int pageNumber);