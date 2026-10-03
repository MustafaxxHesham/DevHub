using DevHub.ActionFilters;
using DevHub.AuthorizationPolicies;
using DevHub.Domain.Models;
using DevHub.DTOS.Posts;
using DevHub.EFCore.ErrorTypes;
using DevHub.Responses;
using DevHub.Services.PostsService;
using DevHub.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevHub.Controllers;

[ApiController]
[Route("api/v1/posts/")]
public class PostsController(IPostService _postService, ILogger<PostsController> _logger) : ControllerBase
{

    [HttpGet("test-minimal")]
    public async Task<ActionResult<Post>> GetOne()
    {
        return Ok(await _postService.Check());
    }

    //  Ensure that the post isn't for premium user subscribed
    [HttpGet("{query:alpha}")]
    [PaginationValidator]
    public async Task<ActionResult<IEnumerable<MinimalPost>>> GetPostsByQuery(string query)
    {
        if (string.IsNullOrEmpty(query) || query.Length == 1)
            return BadRequest("");

        int pageSize = int.Parse(HttpContext.Request.Headers["X-PageSize"]!);

        int pageNumber = int.Parse(HttpContext.Request.Headers["X-PageNumber"]!);

        var keyPagedRequest = KeyPagedRequest<string>.PagedRequestCreate(query, pageSize, pageNumber);

        return Ok((await _postService.SearchPostsAsMinimalAsync(keyPagedRequest)).Value);
    }

    [HttpGet("details/{postId}")]
    public async Task<ActionResult<PostDetailsResponse>> GetDetailedById(string postId)
    {
        var result = await _postService.GetPostInDetailAsync(postId);

        if (!result.IsSuccess)
        {
            if (result.Error.Equals(ResponseMessages.POST_NOT_FOUND))
            {
                return NotFound(result);
            }
            return BadRequest(ResponseMessages.DATA_SENT_MANIPULATED);
        }

        return Ok(result.Value);
    }

    [HttpGet("{tagId}")]
    [PaginationValidator]
    public async Task<ActionResult<IEnumerable<MinimalPost>>> GetByTag(string tagId)
    {
        int pageSize = int.Parse(HttpContext.Request.Headers["X-PageSize"]!);

        int pageNumber = int.Parse(HttpContext.Request.Headers["X-PageNumber"]!);

        var keyedRequest = KeyPagedRequest<string>.PagedRequestCreate(tagId, pageSize, pageNumber);

        var result = await _postService.GetPostsByTagAsync(keyedRequest);

        if (!result.IsSuccess)
        {
            if (result.Error.Equals(DbErrors.NotFoundError.ToString()))
                return NotFound(result.Error);
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("{categoryId}")]
    [PaginationValidator]
    public async Task<ActionResult<IEnumerable<MinimalPost>>> GetByCategory(string categoryId)
    {
        int pageSize = int.Parse(HttpContext.Request.Headers["X-PageSize"]!);

        int pageNumber = int.Parse(HttpContext.Request.Headers["X-PageNumber"]!);

        var keyedRequest = KeyPagedRequest<string>.PagedRequestCreate(categoryId, pageSize, pageNumber);
        
        var result = await _postService.GetPostsByCategoryAsync(keyedRequest);
        
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        return Ok(result.Value);
    }

    [HttpGet("{postId}/recommended")]
    public async Task<ActionResult<IEnumerable<MinimalPost>>> GetRecommendedForPost(string postId)
    {
        if (string.IsNullOrEmpty(postId) || postId.Equals("0"))
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
    [Authorize(Policy = AuthPolicies.AddPostPolicy)]
    public async Task<ActionResult> Post(AddPostRequest request)
    {
        var result = await _postService.CreatePostAsync(request);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetDetailedById), result);

        return BadRequest(result.Error);
    }

    [HttpDelete("{postId}")]
    public async Task<ActionResult> Delete(string postId)
    {
        var result = await _postService.DeletePostAsync(postId);

        if (!result.IsSuccess)
        {
            if (result.Error.Equals(ResponseMessages.POST_NOT_FOUND))
            {
                return NotFound(ResponseMessages.POST_NOT_FOUND);
            }
            return BadRequest(result.Error);
        }

        return NoContent();
    }

    [HttpPatch("bookmarked")]
    public async Task<ActionResult> GetBookmarkedPostForUser()
    {
        var userId = User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return NotFound();
        }

        var keyedRequest = KeyPagedRequest<string>.PagedRequestCreate(userId, 6, 1);

        var result = await _postService.GetBookmarkedPosts(keyedRequest);

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
        // activity score = (likes + comments + shares) / (time since posted in hours + 2)^1.5
        throw new NotImplementedException();
    }

    [HttpGet("count-by-category")]
    public async Task<ActionResult<IEnumerable<CategoryPostsCountResponse>>> GetPostsCountByCategory()
    {
        var result = await _postService.GetPostsCountByCategoryAsync();

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("count-by-Tag")]
    public async Task<ActionResult<IEnumerable<TagPostsCountResponse>>> GetPostsCountByTag()
    {
        var result = await _postService.GetPostsCountByTagAsync();

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("posts-draft/{userId}")]
    public async Task<ActionResult> GetMyPosts(string userId)
    {
        throw new NotImplementedException();
    }

    [HttpGet("posts/by-views")]
    [PaginationValidator]
    public async Task<ActionResult<IEnumerable<MinimalPost>>> GetPostsByViews()
    {
        int pageSize = int.Parse(HttpContext.Request.Headers["X-PageSize"]!);

        int pageNumber = int.Parse(HttpContext.Request.Headers["X-PageNumber"]!);
        
        var result = await _postService.GetPostsOrderedByViews(new DTOS.Commons.PagedRequest(pageSize, pageNumber));
        
        if (!result.IsSuccess)
        { 
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPut]
    public async Task<ActionResult> Edit(string postId)
    {
        throw new NotImplementedException();
    }

    [HttpGet("month-ratio")]
    public async Task<ActionResult> PostsByMonth()
    {
        throw new NotImplementedException();
    }
}

public record class BookmarkPostRequest([FromRoute(Name = "postId")] int PostId, [FromRoute(Name = "x-userId")] string ProtectedUserId);