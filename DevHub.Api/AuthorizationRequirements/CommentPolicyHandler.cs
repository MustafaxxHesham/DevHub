using DevHub.Domain.DataStoreContract;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace DevHub.API.AuthorizationRequirements;
public class CommentPolicyHandler(IDataStore _dataStore) : AuthorizationHandler<CommentPolicyRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CommentPolicyRequirement requirement)
    {
        // Check user has permission or not.
        var userEmail = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

        if (userEmail == null || !await _dataStore.Users.IsEmailExistAsync(userEmail.Value)) {
            context.Fail();
        }


        context.Succeed(requirement);




        // bool x = await _dataStore.Users.HasPermissionAsync("");

        // If user has permission so allow him

        // prevent the user from commenting

        throw new NotImplementedException();
    }
}
