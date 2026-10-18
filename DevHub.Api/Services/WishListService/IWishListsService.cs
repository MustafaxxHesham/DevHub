using DevHub.Domain.Result;
using DevHub.DTOS.WishList;

namespace DevHub.Services.WishListService;
public interface IWishListsService
{
    Task<SimpleResult<bool>> CreateWishListItemAsync(WishListItemRequest request);
    Task<SimpleResult<bool>> DeleteWishListItemAsync(WishListItemRequest request);
}