using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Permissions;
using DevHub.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Claims;
namespace DevHub.API.AuthorizationRequirements;
public class CommentPolicyHandler(IDataStore _dataStore, IDataProtectionProvider provider) : AuthorizationHandler<CommentPolicyRequirement>
{
    private readonly IDataProtector _protector = provider.CreateProtector(ProtectionPurposes.USER_ID_PURPOSE);
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CommentPolicyRequirement requirement)
    {
        var userEmailClaim = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

        if (userEmailClaim == null || !await _dataStore.Users.IsEmailExistAsync(userEmailClaim.Value))
        {
            context.Fail(new AuthorizationFailureReason(this, "You aren't allowed to comment."));
            return;
        }

        var userIdClaims = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

        if (userIdClaims == null)
        {
            context.Fail();
            return;
        }

        var realUserId = int.Parse(_protector.Unprotect(userIdClaims.Value));

        var permissions = await _dataStore.Users.GetUserPermissionsAsync(realUserId);

        if (permissions.Exists(x => x.PermissionName.Equals(PermissionsList.CREATE_COMMENT)))
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail();
    }
}
