using DevHub.DTOS.WishList;
using DevHub.Responses;
using DevHub.Services.WishListService;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers;

[Route("api/v1/wishlists")]
[ApiController]
public class WishListsController(IWishListsService _wishListsService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Post(WishListItemRequest request)
    {
        var result = await _wishListsService.CreateWishListItemAsync(request);
        if (!result.IsSuccess)
        {
            if (result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(result.Error);
            }
            if (result.Error.Equals(ResponseMessages.ACTION_ALREADY_DONE))
            {
                return Conflict();
            }
            return BadRequest(result.Error);
        }
        return Ok();
    }

    [HttpGet]
    public ActionResult Get()
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        throw new NotImplementedException();
    }
}
