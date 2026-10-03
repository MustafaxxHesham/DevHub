using DevHub.Services.AuthenticationService;
using DevHub.Controllers;
using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Result;
using Mapster;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using DevHub.Domain.Models;
using DevHub.Responses;
using DevHub.DTOS.Commons;
using DevHub.Utilities;


namespace DevHub.Services.AdminService;

public class AdminService(IDataStore _dataStore, IDataProtectionProvider _dataProtectionProvider) : IAdminService
{
    private readonly IDataProtector _dataProtector = _dataProtectionProvider.CreateProtector("userId");
    
    public async Task<SimpleResult<bool>> ActivateAccountAsync(string userId)
    {
        var realUserId = int.Parse(_dataProtector.Unprotect(userId));
        var user = await _dataStore.Users.GetByIdAsync(realUserId);

        if (user is null)
            return SimpleResult<bool>.Failure(AuthResultMessages.USER_NOT_FOUND);

        user.IsActive = true;

        _dataStore.Users.UpdateItem(user);
        
        if (await _dataStore.CompleteAsync() > 0)
            return SimpleResult<bool>.Success(true);

        return SimpleResult<bool>.Failure("Error Happenend Must Call The Engineer Now!");
    }

    public async Task<SimpleResult<bool>> DeactivateAccountAsync(string userId)
    {
        var realUserId = handleUserId(userId);
        var user = await _dataStore.Users.GetByIdAsync(realUserId);

        if (user is null)
            return SimpleResult<bool>.Failure(AuthResultMessages.USER_NOT_FOUND);

        user.IsActive = false;

        _dataStore.Users.UpdateItem(user);

        if (await _dataStore.CompleteAsync() > 0)
            return SimpleResult<bool>.Success(true);

        return SimpleResult<bool>.Failure("Error Happenend Must Call The Engineer Now!");
    }

    public async Task<SimpleResult<bool>> DeleteAccountAsync(string userId)
    {
        var realUserId = handleUserId(userId);
        var user = await _dataStore.Users.GetByIdAsync(realUserId);

        if (user is null)
            return SimpleResult<bool>.Failure(AuthResultMessages.USER_NOT_FOUND);

        _dataStore.Users.RemoveItem(user);

        if (await _dataStore.CompleteAsync() > 0)
            return SimpleResult<bool>.Success(true);

        // To be followed.......
        return SimpleResult<bool>.Failure("Error Happenend Must Call The Engineer Now!");

    }

    public async Task<Result<IEnumerable<UsersRequest>>> GetAccountsAsync(int pageSize = 6, int pageNumber = 1)
    {
        TypeAdapterConfig config = new TypeAdapterConfig();

        config.NewConfig<User, UsersRequest>().Map(x => x.RoleName, x => x.Role.RoleName);

        var result = await _dataStore.Users.GetUsersListAsync(pageSize, pageNumber)
                                           .ProjectToType<UsersRequest>(config)
                                           .ToListAsync();

        return Result<IEnumerable<UsersRequest>>.Success(result);
    }

    public async Task<IEnumerable<UsersRequest>> GetAccountsRoleBasedAsync(KeyPagedRequest<int> request)
    {
        //  Critical To be Revisioned!!!!!
        return await _dataStore.Users.GetUsersListRoleBasedAsync(request.PageSize, request.PageNumber, request.Key)
                                     .ProjectToType<UsersRequest>()
                                     .ToListAsync();
    }

    public async Task<int> GetUsersCount()
    {
        return await _dataStore.Users.GetCountAsync();
    }

    public async Task<int> GetOnlineUsersCount()
    {
        throw new NotImplementedException();
    }

    public async Task<Result<List<Permission>>> GetUserPermissionsList(string userId)
    {
        var realUserId = handleUserId(userId);

        var user = await _dataStore.Users.GetByCriteriaFirstAsync(x => x.Id == realUserId, ["Permissions"]);
        
        if (user is null)
            return Result<List<Permission>>.Failure(AuthResultMessages.USER_NOT_FOUND);

        return Result<List<Permission>>.Success(user.Permissions!.ToList());
    }

    public async Task<SimpleResult<bool>> UpdateUserPermissionsAsync(string userId, List<int> permissionsIds)
    {
        var realUserId = handleUserId(userId);

        if (realUserId == -1) { 
            return SimpleResult<bool>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }
        var user = await _dataStore.Users.GetByIdAsync(realUserId);

        if (user is null) {
            return SimpleResult<bool>.Failure(ResponseMessages.USER_NOT_FOUND);
        }

        var dbPermissions = await _dataStore.Permissions.GetAllAsync();

        var newPermissionsList = dbPermissions.Join(permissionsIds, x => x.Id, x => x,(x,y) => new Permission
        {
            Id = x.Id,
            PermissionName = x.PermissionName
        }).ToList();

        user.Permissions = newPermissionsList;

        _dataStore.Users.UpdateItem(user);

        await _dataStore.CompleteAsync();

        return SimpleResult<bool>.Success(true);
    }


    private int handleUserId(string userId)
    {
        return int.Parse(_dataProtector.Unprotect(userId));
    }

    public async Task<IEnumerable<Permission>> GetPermissionsAsync()
    {
        return await _dataStore.Permissions.GetAllAsync();
    }
}
