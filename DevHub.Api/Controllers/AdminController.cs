using DevHub.Services.AdminService;
using DevHub.Domain.Models;
using DevHub.Responses;
using Microsoft.AspNetCore.Mvc;
using DevHub.Domain.DataStoreContract;

namespace DevHub.Controllers;

[ApiController]
[Route("api/v1/admin")]

public class AdminController(IAdminService _adminService, IDataStore _dataStore) : ControllerBase
{
    [HttpGet("users-list")]
    public async Task<ActionResult<IEnumerable<UsersRequest>>> GetAccounts(int pageSize, int pageNumber)
    {
        // To be revisioned
        var result = await _adminService.GetAccountsAsync(pageSize, pageNumber);
        
        if (result.IsSuccess)
            return Ok(result.Value);

        return StatusCode(501);
    }

    [HttpPatch("activate-user/{userId}")]
    public async Task<ActionResult> ActivateUser(int userId)
    {
        // To be revisioned
        var result = await _adminService.ActivateAccountAsync(userId.ToString());
        if (result.IsSuccess)
            return NoContent();
        return StatusCode(501);
    }

    [HttpPatch("deactivate-user/{userId}")]
    public async Task<ActionResult> DeactivateUser(int userId)
    {
        // To be revisioned
        var result = await _adminService.DeactivateAccountAsync(userId.ToString());
        
        if (result.IsSuccess)
            return NoContent();

        return StatusCode(501);
    }

    [HttpDelete("delete-account")]
    public async Task<ActionResult> DeleteAccount(int userId)
    {
        var result = await _adminService.DeleteAccountAsync(userId.ToString());

        if (result.Error.Equals(ResponseMessages.USER_NOT_FOUND))
            return BadRequest();
 
        if (result.IsSuccess)
            return NoContent();

        //To be revisioned.
        return StatusCode(501);
    }

    [HttpGet("users-by-role/{roleId}")]
    public async Task<ActionResult<IEnumerable<UsersRequest>>> GetUsersByRole(int roleId, int pageSize, int pageNumber)
    {
        return Ok(await _adminService.GetAccountsRoleBasedAsync(roleId, pageSize, pageNumber));
    }

    [HttpPatch("blocking-user-commenting")]
    public async Task<ActionResult> BlockUserFromCommenting(string userId)
    {
        throw new NotImplementedException();
    }

    [HttpGet("retrieve-permissions")]
    public async Task<ActionResult<List<Permission>>> GetPermissions()
    {
        return Ok(await _adminService.GetPermissionsAsync());
    }

    //[HttpPatch("block-user-posting")]
    //public async Task<ActionResult> BlockPosting(int userId)
    //{
    //    throw new NotImplementedException();
    //}

    //[HttpPatch("unblock-user-posting")]
    //public async Task<ActionResult> RevertBlockPosting(int userId)
    //{
    //    throw new NotImplementedException();
    //}

    [HttpGet("tags")]
    public async Task<ActionResult<Tag>> GetTags()
    {
        throw new NotImplementedException();
    }

    [HttpPatch("update-permissions")]
    public async Task<ActionResult> UpdateUserPermissions(string userId, List<int> permissionsIds)
    {
        var result = await _adminService.UpdateUserPermissionsAsync(userId, permissionsIds);

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

    // Comments Mngmnt
    /*
    [HttpGet("roles")]
    public async Task<ActionResult<IEnumerable<RolesRequest>>> GetAllRolesAsync()
    {
        var roles = await _dataStore.Roles.GetAllAsync();
        var rolesList = roles.Select(r => new RolesRequest(r.Id, r.RoleName));   
        return Ok(rolesList);
    }

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UsersRequest>>> GetUsers()
    {
        var result = await _dataStore.Users.GetAllAsync();
        throw new NotImplementedException();
    }

    */
}


public record class RolesRequest(int Id, string RoleName);
public record class UsersRequest(int Id,
                                 string FirstName,
                                 string LastName,
                                 string RoleName, 
                                 bool IsEmailVerified, 
                                 bool IsLocked);