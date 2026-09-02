using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Result;
using DevHub.DTOS.Users;
using DevHub.EFCore.ErrorTypes;
using DevHub.Utilities;
using Microsoft.AspNetCore.DataProtection;

namespace DevHub.Services.UsersService;

public class UsersService(IDataStore _dataStore,
                          IDataProtectionProvider provider,
                          ILogger<UsersService> _logger) : IUsersService
{
    private readonly IDataProtector _dataProtector = provider.CreateProtector(ProtectionPurposes.USER_ID_PURPOSE);

    public async Task<Result<UserProfile>> GetMyProfile(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<SimpleResult<int>> GetUserFollowersCountAsync(string userId)
    {
        int uid = getUserIdInt(userId);

        if (uid == -1)
        {
            return SimpleResult<int>.Failure("Error Happened For Server");
        }
        
        int followersCount = await _dataStore.Followers.GetCountWithCriteriaAsync(uf => uf.FollowedUserId == uid);

        return SimpleResult<int>.Success(followersCount);
    }

    public async Task<SimpleResult<bool>> UpdateUserAsync(EditUserRequest request, string webRootPath)
    {
        int uid = getUserIdInt(request.UserId);

        if (uid == -1)
        {
            return SimpleResult<bool>.Failure("Error Happened For Server");
        }
        var user = await _dataStore.Users.GetByCriteriaFirstAsync(u => u.Id == uid);

        if (user is null)
        {
            return SimpleResult<bool>.Failure(DbErrors.NotFoundError.ToString());
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
