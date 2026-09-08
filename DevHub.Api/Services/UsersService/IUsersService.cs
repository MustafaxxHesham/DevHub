using DevHub.Domain.Result;
using DevHub.DTOS.Commons;
using DevHub.DTOS.Users;
using DevHub.Utilities;
namespace DevHub.Services.UsersService;
public interface IUsersService
{
    Task<Result<UserRatioCountResponse>> GetUsersCountMonthlyAsync();
    Task<SimpleResult<int>> GetUserFollowingsCountAsync(string userId);
    Task<SimpleResult<int>> GetUserFollowersCountAsync(string userId);
    Task<Result<PagedResponse<UserResultResponse>>> GetUserFollowersAsync(string userId, int pageSize, int pageNumber);
    Task<Result<PagedResponse<UserResultResponse>>> GetUserFollowingsAsync(string userId, int pageSize, int pageNumber);
    Task<SimpleResult<bool>> UpdateUserAsync(EditUserRequest request, string webRootPath);
    Task<Result<UserProfileResponse>> GetMyProfile(string userId);
    Task<PagedResponse<UserResultResponse>> SearchUsersAsync(PagedSearchRequest request);
    Task<SimpleResult<bool>> FollowUserAsync(string userId, string userToFollowId);
    Task<SimpleResult<bool>> UnfollowUserAsync(string userId, string userToUnfollowId);
}