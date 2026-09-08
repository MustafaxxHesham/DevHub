using DevHub.Controllers;
using DevHub.Domain.Models;
using DevHub.Domain.Result;
namespace DevHub.Services.AdminService;
public interface IAdminService
{
    Task<SimpleResult<bool>> DeactivateAccountAsync(string userId);
    Task<SimpleResult<bool>> ActivateAccountAsync(string userId);
    Task<Result<IEnumerable<UsersRequest>>> GetAccountsAsync(int pageSize = 6, int pageNumber = 1);
    Task<IEnumerable<UsersRequest>> GetAccountsRoleBasedAsync(int roleId, int pageSize = 6, int pageNumber = 1);
    Task<SimpleResult<bool>> DeleteAccountAsync(string userId);
    Task<Result<List<Permission>>> GetUserPermissionsList(string userId);
    Task<IEnumerable<Permission>> GetPermissionsAsync();
    Task<SimpleResult<bool>> UpdateUserPermissionsAsync(string userId, List<int> permissionsIds);
    Task<int> GetOnlineUsersCount();
    Task<int> GetUsersCount();
}