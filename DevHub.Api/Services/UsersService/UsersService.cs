using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Models;
using DevHub.Domain.Result;
using DevHub.DTOS.Commons;
using DevHub.DTOS.Users;
using DevHub.Responses;
using DevHub.Utilities;
using Microsoft.AspNetCore.DataProtection;

namespace DevHub.Services.UsersService;

public class UsersService(IDataStore _dataStore, IDataProtectionProvider provider, ILogger<UsersService> _logger) : IUsersService
{
    private readonly IDataProtector _dataProtector = provider.CreateProtector(ProtectionPurposes.USER_ID_PURPOSE);

    public async Task<SimpleResult<int>> GetUserFollowersCountAsync(string userId)
    {
        int uid = getUserIdInt(userId);

        if (uid == -1)
        {
            return SimpleResult<int>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }
        
        if (!await _dataStore.Users.IsExistAsync(uid))
        {
            return SimpleResult<int>.Failure(ResponseMessages.USER_NOT_FOUND);
        }

        int followersCount = await _dataStore.Followers.GetCountWithCriteriaAsync(uf => uf.FollowedUserId == uid);

        return SimpleResult<int>.Success(followersCount);
    }

    public async Task<SimpleResult<int>> GetUserFollowingsCountAsync(string userId)
    {
        int uid = getUserIdInt(userId);

        if (uid == -1)
        {
            return SimpleResult<int>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }
        
        int followersCount = await _dataStore.Followers.GetCountWithCriteriaAsync(uf => uf.FollowerUserId == uid);

        return SimpleResult<int>.Success(followersCount);
    }
    
    public async Task<SimpleResult<bool>> UpdateUserAsync(EditUserRequest request, string webRootPath)
    {
        int uid = getUserIdInt(request.UserId);

        if (uid == -1)
        {
            return SimpleResult<bool>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }
        var user = await _dataStore.Users.GetByCriteriaFirstAsync(u => u.Id == uid);

        if (user is null)
        {
            return SimpleResult<bool>.Failure(ResponseMessages.USER_NOT_FOUND);
        }

        if (!string.IsNullOrEmpty(request.FirstName) && request.FirstName.Trim().Length <= 1)
        {
            user.FirstName = request.FirstName;
        }

        if (!string.IsNullOrEmpty(request.LastName) && request.LastName.Trim().Length <= 1)
        {
            user.LastName = request.LastName;
        }

        if (!string.IsNullOrEmpty(request.Bio) && request.Bio.Trim().Length <= 1)
        {
            user.Bio = request.Bio;
        }

        if (request.ProfileImage is not null && request.ProfileImage.Length > 0)
        {
            var result = await UploadFile.UploadImageFileToServerAsync(request.ProfileImage, UploadFilePurpose.USER_PROFILE_IMG, webRootPath);
            if (string.IsNullOrEmpty(result))
            {
                
            }
            else
            {
                user.ProfileImageUrl = result;
            }
        }

        _dataStore.Users.UpdateItem(user);

        await _dataStore.CompleteAsync();

        return SimpleResult<bool>.Success(true);

    }

    public async Task<SimpleResult<bool>> FollowUserAsync(string userId, string userToFollowId)
    {
        var uid = getUserIdInt(userId);
        if (uid == -1)
        {
            return SimpleResult<bool>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }

        var userToFollowRealId = getUserIdInt(userToFollowId);
        if (uid == -1)
        {
            return SimpleResult<bool>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }

        if (!await _dataStore.Users.IsExistAsync(userToFollowRealId) || !await _dataStore.Users.IsExistAsync(uid))
        {
            return SimpleResult<bool>.Failure(ResponseMessages.USER_NOT_FOUND);
        }

        var userFollower = new UserFollower
        {
            FollowerUserId = uid,
            FollowedUserId = userToFollowRealId
        };

        if (await _dataStore.Followers.IsExistWithCriteriaAsync(uf => uf.FollowerUserId == uid && uf.FollowedUserId == userToFollowRealId))
        {
            return SimpleResult<bool>.Failure(ResponseMessages.USER_ALREADY_FOLLOWED);
        }

        await _dataStore.Followers.AddAsync(userFollower);

        return SimpleResult<bool>.Success(true);
    }
    
    public async Task<SimpleResult<bool>> UnfollowUserAsync(string userId, string userToFollowId)
    {
        var uid = getUserIdInt(userId);

        if (uid == -1)
        {
            return SimpleResult<bool>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }

        var userToFollowRealId = getUserIdInt(userToFollowId);
        
        if (uid == -1)
        {
            return SimpleResult<bool>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }

        if (!await _dataStore.Users.IsExistAsync(userToFollowRealId) || !await _dataStore.Users.IsExistAsync(uid))
        {
            return SimpleResult<bool>.Failure(ResponseMessages.USER_NOT_FOUND);
        }

        var uf = await _dataStore.Followers.GetByCriteriaFirstAsync(uf => uf.FollowerUserId == uid && uf.FollowedUserId == userToFollowRealId);

        if (uf != null)
        {
            return SimpleResult<bool>.Failure(ResponseMessages.USER_ALREADY_NOT_FOLLOWED);
        }


        _dataStore.Followers.RemoveItem(uf);

        await _dataStore.CompleteAsync();

        return SimpleResult<bool>.Success(true);

    }
    
    public async Task<Result<UserProfileResponse>> GetMyProfileAsync(string userId)
    {
        var uid = getUserIdInt(userId);// Check for id

        if (uid == -1)
        {
            return Result<UserProfileResponse>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }

        var (user, userFollowers, totalViewsCount) = await _dataStore.Users.GetUserProfileAsync(1);

        if (user is null)
        {
            return Result<UserProfileResponse>.Failure(ResponseMessages.USER_NOT_FOUND);
        }

        var userProfile = new UserProfileResponse
        (
            FullName:$"{user.FirstName} {user.LastName}",
            Email:user.Email,
            ProfileImageUrl:user.ProfileImageUrl,
            FollowersCount:userFollowers.Count(u => u.FollowedUserId == uid),
            FollowingCount:userFollowers.Count(u => u.FollowerUserId == uid),
            PostsViews:totalViewsCount,
            JobTitle: user.JobTitle,
            Bio:user.Bio
        );

        return Result<UserProfileResponse>.Success(userProfile);
    }

    public async Task<Result<UserRatioCountResponse>> GetUsersCountMonthlyAsync()
    {
        var result = await _dataStore.Users.GetIncreasedUsersRatioMonthly();
        var values = new UserRatioCountResponse((int)result[0], result[1]);
        return Result<UserRatioCountResponse>.Success(values);
    }

    public async Task<PagedResponse<UserResultResponse>> SearchUsersAsync(PagedSearchRequest request)
    {
        var data = await _dataStore.Users.SearchUsersAsync(request.searchKey, request.pageSize, request.pageNumber);

        var valueList = convertUserToUserResult(data.ValueList);

        PagedResponse<UserResultResponse> response = new PagedResponse<UserResultResponse>(valueList, data.CurrentPage, data.PageSize, data.TotalCount);

        return response;
    }

    public async Task<Result<PagedResponse<UserResultResponse>>> GetUserFollowingsAsync(string userId, int pageSize, int pageNumber)
    {
        var realUserId = getUserIdInt(userId);
        
        if (realUserId == -1)
        {
            return Result<PagedResponse<UserResultResponse>>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }

        if (!await _dataStore.Users.IsExistAsync(realUserId))
        {
            return Result<PagedResponse<UserResultResponse>>.Failure(ResponseMessages.USER_NOT_FOUND);
        }

        var data = await _dataStore.Users.GetMyFollowings(realUserId, pageSize, pageNumber);

        var values = PagedResponse<UserResultResponse>.Create(convertUserToUserResult(data.ValueList), data.PageSize, data.CurrentPage, data.TotalCount);

        return Result<PagedResponse<UserResultResponse>>.Success(values);

    }

    public async Task<Result<PagedResponse<UserResultResponse>>> GetUserFollowersAsync(string userId, int pageSize, int pageNumber)
    {
        var realUserId = getUserIdInt(userId);
        
        if (realUserId == -1)
        {
            return Result<PagedResponse<UserResultResponse>>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }

        if (!await _dataStore.Users.IsExistAsync(realUserId))
        {
            return Result<PagedResponse<UserResultResponse>>.Failure(ResponseMessages.USER_NOT_FOUND);
        }

        var data = await _dataStore.Users.GetMyFollowers(realUserId, pageSize, pageNumber);

        var values = PagedResponse<UserResultResponse>.Create(convertUserToUserResult(data.ValueList), data.PageSize, data.CurrentPage, data.TotalCount);

        return Result<PagedResponse<UserResultResponse>>.Success(values);

    }

    public async Task<PagedResponse<UserResultResponse>> GetUsersListAsync(int pageSize, int pageNumber)
    {
        var data = await _dataStore.Users.GetPaginatedAsync(pageSize, pageNumber, x => x.Id);

        var valueList = convertUserToUserResult(data.ValueList);

        PagedResponse<UserResultResponse> response = new PagedResponse<UserResultResponse>(valueList, data.CurrentPage, data.PageSize, data.TotalCount);

        return response;
    }

    public async Task<PagedResponse<UserResultResponse>> GetUsersListByGoogleAsync(int pageSize, int pageNumber)
    {
        var externalLoginsList = await _dataStore.Users.GetUsersGoogleAuth(pageSize, pageNumber);
        
        if (externalLoginsList == null || externalLoginsList.ValueList.Count <= 0)
        {
            return null;
        }

        var data = new List<User>();
        
        foreach (var item in externalLoginsList.ValueList)
        {
            data.Add(item.User);
        }
        var userResponseList = convertUserToUserResult(data);

        return PagedResponse<UserResultResponse>.Create(userResponseList, externalLoginsList.PageSize, externalLoginsList.CurrentPage, externalLoginsList.TotalPages);
    }

    public async Task<PagedResponse<UserResultResponse>> GetUsersListByGithubAsync(int pageSize, int pageNumber)
    {
        var externalLoginsList = await _dataStore.Users.GetUsersGitHubAuth(pageSize, pageNumber);

        if (externalLoginsList == null || externalLoginsList.ValueList.Count <= 0)
        {
            return null;
        }

        var data = new List<User>();

        foreach (var item in externalLoginsList.ValueList)
        {
            data.Add(item.User);
        }
        var userResponseList = convertUserToUserResult(data);

        return PagedResponse<UserResultResponse>.Create(userResponseList, externalLoginsList.PageSize, externalLoginsList.CurrentPage, externalLoginsList.TotalPages);
    }

    public async Task<Dictionary<string, int>> GetUsersAuthRatioAsync()
    {
        return await _dataStore.Users.GetAuthRatio();
    }

    private List<UserResultResponse> convertUserToUserResult(List<User> users)
    {
        List<UserResultResponse> result = new();

        foreach (var user in users) {
            result.Add(new UserResultResponse(user.FirstName + " " + user.LastName, user.ProfileImageUrl, user.JobTitle));
        }

        return result;
    }
    private int getUserIdInt(string id)
    { 
        try
        {
            return int.Parse(_dataProtector.Unprotect(id));
        } catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return -1;
        }
    }

}
