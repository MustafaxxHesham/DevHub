using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Models;
using DevHub.Domain.Result;
using DevHub.DTOS.WishList;
using DevHub.Responses;
using DevHub.Utilities;

namespace DevHub.Services.WishListService;

public class WishListsService(IDataStore _dataStore, ProtectionHandler _protectionHandler) : IWishListsService
{
    public async Task<SimpleResult<bool>> CreateWishListItemAsync(WishListItemRequest request)
    {
        var userId = _protectionHandler.GetRealUserId(request.UserId);
        if (userId == -1 || !await _dataStore.Users.IsExistAsync(userId))
        {
            return SimpleResult<bool>.Failure(ResponseMessages.USER_NOT_FOUND);
        }
        var courseId = _protectionHandler.GetRealCourseId(request.CourseId);
        if (courseId == -1 || !await _dataStore.Courses.IsExistAsync(userId))
        {
            return SimpleResult<bool>.Failure(ResponseMessages.COURSE_NOT_FOUND);
        }

        if (await _dataStore.WishLists.IsExistWithCriteriaAsync(w => w.CourseId == courseId && w.UserId == userId))
        {
            return SimpleResult<bool>.Failure(ResponseMessages.ACTION_ALREADY_DONE);
        }
        var wishlist = new WishList { UserId = userId, CourseId = courseId };

        await _dataStore.WishLists.AddAsync(wishlist);
        await _dataStore.CompleteAsync();

        return SimpleResult<bool>.Success(true);
    }

    public async Task<SimpleResult<bool>> DeleteWishListItemAsync(WishListItemRequest request)
    {
        var userId = _protectionHandler.GetRealUserId(request.UserId);

        if (userId == -1 || !await _dataStore.Users.IsExistAsync(userId))
        {
            return SimpleResult<bool>.Failure(ResponseMessages.USER_NOT_FOUND);
        }
        var courseId = _protectionHandler.GetRealCourseId(request.CourseId);
        
        if (courseId == -1 || !await _dataStore.Courses.IsExistAsync(userId))
        {
            return SimpleResult<bool>.Failure(ResponseMessages.COURSE_NOT_FOUND);
        }

        var wishlist = await _dataStore.WishLists.GetByCriteriaFirstAsync(w => w.CourseId == courseId && w.UserId == userId);
        
        if (wishlist is null)
        {
            return SimpleResult<bool>.Failure(ResponseMessages.ACTION_ALREADY_DONE);
        }

        _dataStore.WishLists.RemoveItem(wishlist);
        
        await _dataStore.CompleteAsync();

        return SimpleResult<bool>.Success(true);
    }
}