using DevHub.Domain.Result;
using DevHub.DTOS.Commons;
using DevHub.DTOS.Users;
using DevHub.Utilities;
namespace DevHub.Services.UsersService;
public interface IUsersService
{
    Task<Dictionary<string, int>> GetUsersAuthRatioAsync();
    Task<SimpleResult<int>> GetUserFollowingsCountAsync(string userId);
    Task<SimpleResult<int>> GetUserFollowersCountAsync(string userId);
    Task<SimpleResult<bool>> UpdateUserAsync(EditUserRequest request, string webRootPath);
    Task<SimpleResult<bool>> FollowUserAsync(string userId, string userToFollowId);
    Task<SimpleResult<bool>> UnfollowUserAsync(string userId, string userToUnfollowId);
    Task<PagedResponse<UserResultResponse>> GetUsersListAsync(int pageSize, int pageNumber);
    Task<PagedResponse<UserResultResponse>> GetUsersListByGoogleAsync(int pageSize, int pageNumber);
    Task<PagedResponse<UserResultResponse>> GetUsersListByGithubAsync(int pageSize, int pageNumber);
    Task<PagedResponse<UserResultResponse>> SearchUsersAsync(PagedSearchRequest request);
    Task<Result<UserRatioCountResponse>> GetUsersCountMonthlyAsync();
    Task<Result<UserProfileResponse>> GetMyProfileAsync(string userId);
    Task<Result<PagedResponse<UserResultResponse>>> GetUserFollowersAsync(string userId, int pageSize, int pageNumber);
    Task<Result<PagedResponse<UserResultResponse>>> GetUserFollowingsAsync(string userId, int pageSize, int pageNumber);
}