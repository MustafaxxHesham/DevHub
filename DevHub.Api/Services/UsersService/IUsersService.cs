using DevHub.Domain.Result;
using DevHub.DTOS.Users;
namespace DevHub.Services.UsersService;
public interface IUsersService
{
    Task<SimpleResult<int>> GetUserFollowersCountAsync(string userId);
    Task<SimpleResult<bool>> UpdateUserAsync(EditUserRequest request, string webRootPath);
    Task<Result<UserProfile>> GetMyProfile(string userId);
}

public record class UserProfile( 
        string FullName, 
        string Email, 
        string? ProfileImageUrl, 
        int FollowersCount, 
        int FollowingCount,
        int PostsViews);