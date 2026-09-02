using DevHub.ActionFilters;
using DevHub.Domain.Models;
using DevHub.DTOS.Posts;
using DevHub.Services.PostsService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;

namespace DevHub.Controllers;

[ApiController]
[Route("api/v1/posts/")]
public class PostsController(IPostService _postService, ILogger<PostsController> _logger) : ControllerBase
{
    //  Ensure that the post isn't for preimum user subscribed
    [HttpGet("{query:alpha}")]
    public async Task<ActionResult<IEnumerable<MinimalPost>>> GetPostsByQuery(string query)
    {
        if (string.IsNullOrEmpty(query) || query.Length == 1)
            return BadRequest("");

        var req = new SearchPostsRequest(query, 1, 1);

        var result = await _postService.SearchPostsAsMinimalAsync(req);

        //if (result.IsSuccess)
        return Ok(result.Value);
    }

    [HttpGet("{postId}")]
    public async Task<ActionResult<PostDetailsResponse>> GetDetailedById(int postId)
    {
        if (postId <= 0)
            return BadRequest();

        var result = await _postService.GetPostInDetailAsync(postId);

        if (result.IsSuccess)
            return Ok(result.Value);

        return NotFound(result);
    }

    [HttpGet("{tagId}")]
    public async Task<ActionResult<IEnumerable<MinimalPost>>> GetByTag(int tagId)
    {
//        var isExist = await _dataStore.Categories.IsExistAsync(request.CategoryId);
        int pageSizeAsInt = 0;
        int pageNumberAsInt = 0;

        const int MAX_PAGE_SIZE = 30;
        
/*        if (!isExist)
            return NotFound("Category isn't provided.");*/
        // Revision 
        if (HttpContext.Request.Headers.TryGetValue("X-PageSize", out StringValues pageSize) && HttpContext.Request.Headers.TryGetValue("X-PageNumber", out StringValues pageNumber))
        {
            pageSizeAsInt = int.Parse(pageSize);
            pageNumberAsInt = int.Parse(pageNumber);

            if (pageNumberAsInt < 0 || pageSizeAsInt < 0)
                return BadRequest();
        }

        if (pageSizeAsInt == 0 || pageNumberAsInt == 0)
            return BadRequest("Add both page size & page number.");

        var result = await _postService.GetPostsByCategoryAsync(tagId.ToString(), pageNumberAsInt, pageSizeAsInt);
            
        return Ok(result.Value);
    }

    [HttpGet("{categoryId}")]
    public async Task<ActionResult<IEnumerable<MinimalPost>>> GetByCategory(int categoryId)
    {
//        var isExist = await _dataStore.Categories.IsExistAsync(request.CategoryId);
        int pageSizeAsInt = 0;
        int pageNumberAsInt = 0;

        const int MAX_PAGE_SIZE = 30;
        
/*        if (!isExist)
            return NotFound("Category isn't provided.");*/
        // Revision 
        if (HttpContext.Request.Headers.TryGetValue("X-PageSize", out StringValues pageSize) && HttpContext.Request.Headers.TryGetValue("X-PageNumber", out StringValues pageNumber))
        {
            pageSizeAsInt = int.Parse(pageSize);
            pageNumberAsInt = int.Parse(pageNumber);

            if (pageNumberAsInt < 0 || pageSizeAsInt < 0)
                return BadRequest();
        }

        if (pageSizeAsInt == 0 || pageNumberAsInt == 0)
            return BadRequest("Add both page size & page number.");

        //rev
        var result = await _postService.GetPostsByCategoryAsync(categoryId.ToString(), pageNumberAsInt, pageSizeAsInt);
            
        return Ok(result.Value);
    }

    [HttpGet("{postId}/recommended")]
    public async Task<ActionResult<IEnumerable<MinimalPost>>> GetRecommendedForPost(int postId)
    {
        if (postId <= 0)
            return BadRequest();

//        var httpResult = await _httpClientFactory.CreateClient("PostScore").GetFromJsonAsync<IEnumerable<PostScore>>("");
//        HashSet<PostScore> postIds = httpResult!.ToHashSet();
        

        throw new NotImplementedException();

    }

    [HttpGet("For-You")]
    public async Task<ActionResult<IEnumerable<MinimalPost>>> PostsForUser()
    {
        var userIdInStr = User.Claims.FirstOrDefault(uc => uc.Type == ClaimTypes.NameIdentifier);
        
        if (userIdInStr == null)
            return BadRequest();

        int userId = int.Parse(userIdInStr.Value ?? "0");

        if (userId == 0)
            return Unauthorized();  // Redirected To General Feed

        var result = await _postService.GetForYouPosts(userId);

        return Ok(result.Value);
    }

    [HttpPost]
    [PostCategoryEnsure]
    public async Task<ActionResult> Post([FromBody] AddPostRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        //  Validate Post.

        //  Ensure Image State

        var result = await _postService.CreatePostAsync(request);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetDetailedById), result);

        return BadRequest(result.Error);
    }

    [HttpDelete("{postId}")]
    public async Task<ActionResult> Delete(int postId)
    {
        var result = await _postService.DeletePostAsync(postId);

        if (result.IsSuccess)
            return NoContent();

        return BadRequest(result.Error);
    }

    [HttpPatch("bookmarked")]
    public async Task<ActionResult> GetBookmarkedPostForUser()
    {
        var userId = User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return NotFound();
        }

        var result = await _postService.GetBookmarkedPosts(int.Parse(userId));

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPatch("{postId}/bookmark/{userId}")]
    public async Task<ActionResult> BookmarkPostForUser(BookmarkPostRequest request)
    {
        var stringUserId = "";// _dataProtector.Unprotect(request.ProtectedUserId);

        if (stringUserId is null)
            return BadRequest(new { Error = "Invalid user ID." });

        var userId = int.Parse(stringUserId);

        throw new NotImplementedException();

    }

    [HttpGet("ranked")]
    public async Task<ActionResult> GetPostsTopRanked()
    {
        throw new NotImplementedException();
    }
}

public record class BookmarkPostRequest([FromRoute(Name = "postId")] int PostId, [FromRoute(Name = "x-userId")] string ProtectedUserId);

/*
 SELECT TOP(1) [p].[Id], [c].[Name] AS [Category], [p].[Title], COALESCE([u].[FirstName], N'') + N' ' + COALESCE([u].[LastName], N'') AS [AuthorName], SUBSTRING([p].[Content], 0 + 1, 60) AS [SecondaryText], [u].[ProfileImageUrl] AS [AuthorImageProfileUrl], [p].[MainImageUrl] AS [FeatureImageUrl]
      FROM [Posts] AS [p]
      INNER JOIN [Categories] AS [c] ON [p].[CategoryId] = [c].[Id]
      LEFT JOIN [Users] AS [u] ON [p].[AuthorId] = [u].[Id]
      WHERE [p].[Id] = 4 AND [p].[IsDeleted] = CAST(0 AS bit) 

SELECT TOP(1) [p].[Id], [c].[Name] AS [Category], [p].[Title], COALESCE([u].[FirstName], N'') + N' ' + COALESCE([u].[LastName], N'') AS [AuthorName], SUBSTRING([p].[Content], 0 + 1, 60) AS [SecondaryText], [u].[ProfileImageUrl] AS [AuthorImageProfileUrl], [p].[MainImageUrl] AS [FeatureImageUrl]
      FROM [Posts] AS [p]
      INNER JOIN [Categories] AS [c] ON [p].[CategoryId] = [c].[Id]
      LEFT JOIN [Users] AS [u] ON [p].[AuthorId] = [u].[Id]
      WHERE [p].[Id] = 4 AND [p].[IsDeleted] = CAST(0 AS bit)
*/