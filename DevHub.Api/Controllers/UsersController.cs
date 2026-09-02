using DevHub.DTOS.Users;
using DevHub.Services.UsersService;
using Microsoft.AspNetCore.Mvc;
namespace DevHub.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController(IUsersService _userService) : ControllerBase
{
    [HttpGet("{userId}/profile")]
    public async Task<ActionResult> GetMyProfile(string userId)
    {
        throw new NotImplementedException();
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


    [HttpGet]
    public async Task<ActionResult> MyFollowingAuthors(string userId) 
    {
        throw new NotImplementedException();
    }


    [HttpPatch]
    public async Task<ActionResult> UpdateUser(EditUserRequest request, [FromServices] IWebHostEnvironment _env)
    {
        string webRootPath = _env.WebRootPath;
        var result = await _userService.UpdateUserAsync(request, webRootPath);
        if (result.IsSuccess)
            return Ok(result.Value);
        throw new NotImplementedException();
    }
}