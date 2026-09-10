using DevHub.ActionFilters;
using DevHub.DTOS.Commons;
using DevHub.DTOS.Users;
using DevHub.Responses;
using DevHub.Services.UsersService;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace DevHub.Controllers;

/* UsersModule,    ReportsModule, TagsModule, CategoriesModule, CommentsModule, PostsModule [1]*/
/* CoursesModule,  SubscriptionsModule--> AuthorizationRules, WalletModule                  [2]*/
/* Angular Front-End Using NgRx for state management                                        [3]*/

[ApiController]
[Route("api/v1/users")]
public class UsersController(IUsersService _userService) : ControllerBase
{
    [HttpGet("{userId}/profile")]
    public async Task<ActionResult> GetMyProfile(string userId)
    {
        var result = await _userService.GetMyProfileAsync(userId);
        
        if (!result.IsSuccess)
        {
            return NotFound();
        }

        return Ok(result.Value);
    }

    [HttpDelete("{userId}")]
    public async Task<ActionResult> DeleteMyAccount(string userId)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{userId}/followers")]
    public async Task<ActionResult> GetUserFollowers(string userId)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{userId}/followings")]
    public async Task<ActionResult> MyFollowingAuthorsCount(string userId) 
    {
        var result = await _userService.GetUserFollowingsCountAsync(userId);
        if (!result.IsSuccess)
        {
            if (result.Error.Equals(ResponseMessages.USER_NOT_FOUND))
            {
                return NotFound(result.Error);
            }
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }

    [HttpGet("Users-Count-Monthly")]
    public async Task<ActionResult<UserRatioCountResponse>> GetUsersCountMonthly()
    {
        var result = await _userService.GetUsersCountMonthlyAsync();
        return Ok(result.Value);
    }

    [HttpGet("search/{query}")]
    public async Task<ActionResult> SearchUsers(string query)
    {
        int pageSize = int.Parse(HttpContext.Request.Headers["X-PageSize"]!);
        int pageNumber = int.Parse(HttpContext.Request.Headers["X-PageNumber"]!);
        var result = await _userService.SearchUsersAsync(new PagedSearchRequest(query, pageSize, pageNumber));
        return Ok(result);
    }

    [HttpPatch]
    public async Task<ActionResult> UpdateUser(EditUserRequest request)
    {
        var webRootPath = HttpContext.RequestServices.GetService<IWebHostEnvironment>()?.WebRootPath;

        var result = await _userService.UpdateUserAsync(request, webRootPath);
        
        if (!result.IsSuccess)
        {
            if (result.Error.Equals(ResponseMessages.USER_NOT_FOUND))
            {
                return NotFound(result.Error);
            }
            return BadRequest(result.Error);
        }
        return NoContent();        
    }

    [HttpPost("follow/{userToFollowId}")]
    public async Task<ActionResult> FollowUser(string userToFollowId)
    {
        var signedInUserId = User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        if (signedInUserId == null) {
            return Unauthorized();
        }

        var result = await _userService.FollowUserAsync(signedInUserId, userToFollowId);

        if (!result.IsSuccess) {
            if (result.Error.Equals(ResponseMessages.USER_NOT_FOUND))
            {
                return NotFound(result.Error);
            }
            if (result.Error.Equals(ResponseMessages.USER_ALREADY_FOLLOWED))
            {
                return Conflict(result.Error);
            }
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpPost("unfollow/{userToFollowId}")]
    public async Task<ActionResult> UnFollow(string userToFollowId)
    {
        var signedInUserId = User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        if (signedInUserId == null)
        {
            return Unauthorized();
        }

        var result = await _userService.UnfollowUserAsync(signedInUserId, userToFollowId);

        if (!result.IsSuccess)
        {
            if (result.Error.Equals(ResponseMessages.USER_NOT_FOUND))
            {
                return NotFound(result.Error);
            }
            if (result.Error.Equals(ResponseMessages.USER_ALREADY_NOT_FOLLOWED))
            {
                return Conflict(result.Error);
            }
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpGet("users-authenticated-google")]
    [PaginationValidator]
    public async Task<ActionResult> GetUsersByAuthedGoogle()
    {
        int pageSize = int.Parse(HttpContext.Request.Headers["X-PageSize"]!);
        int pageNumber = int.Parse(HttpContext.Request.Headers["X-PageNumber"]!);
        return Ok(await _userService.GetUsersListByGoogleAsync(pageSize, pageNumber));
    }

    [HttpGet("users-authenticated-github")]
    [PaginationValidator]
    public async Task<ActionResult> GetUsersByAuthedGitHub()
    {
        int pageSize = int.Parse(HttpContext.Request.Headers["X-PageSize"]!);
        int pageNumber = int.Parse(HttpContext.Request.Headers["X-PageNumber"]!);
        return Ok(await _userService.GetUsersListByGithubAsync(pageSize, pageNumber));
    }

    [HttpGet("users-ratio")]
    public async Task<ActionResult> GetRatio()
    {
        return Ok(await _userService.GetUsersAuthRatioAsync());
    }
}