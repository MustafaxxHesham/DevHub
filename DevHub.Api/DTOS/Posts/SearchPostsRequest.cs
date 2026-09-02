using Microsoft.AspNetCore.Mvc;

namespace DevHub.DTOS.Posts;

public record class SearchPostsRequest (
    [FromRoute(Name = "Query")]string Query,
    [FromHeader(Name = "X-PageSize")] int PageSize = 6,
    [FromHeader(Name = "X-PageNumber")] int PageNumber = 1);
