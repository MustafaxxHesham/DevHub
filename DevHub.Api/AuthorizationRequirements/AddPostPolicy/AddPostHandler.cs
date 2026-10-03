using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Claims;

namespace DevHub.AuthorizationRequirements.AddPostPolicy;
public class AddPostHandler(IDataStore _dataStore, IDataProtectionProvider provider) : 
    AuthorizationHandler<AddPostRequirement>, IAuthorizationHandler
{
    private readonly IDataProtector _protector = provider.CreateProtector("UserId");
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AddPostRequirement requirement)
    {
        if (context.User is null)
        {
            context.Fail();
        }

        var userSubscription = context?.User?.FindFirst(nameof(SubscriptionPlans));

        if (userSubscription == null)
        {
            context?.Fail();
        }

        var userIdClaim = context?.User?.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null && string.IsNullOrEmpty(userIdClaim?.Value))
        {
            context?.Fail();
        }

        var userIdAsString = _protector.Unprotect(userIdClaim?.Value);

        if(int.TryParse(userIdAsString, out int userId))
        {
            // if user is connected and deletes the account in same time...
            if (!await _dataStore.Users.IsExistAsync(userId))
            {
                var authorizationFailureReason = new AuthorizationFailureReason(this, "User not found.");
                context?.Fail(authorizationFailureReason);
            }

            var lastPostDate = await _dataStore.Posts.GetLastPostDateForUserAsync(userId);

            if (userSubscription!.Value.Equals(SubscriptionPlans.Free.ToString()))
            {
                if (DateTime.UtcNow.Year == lastPostDate.Year && (DateTime.UtcNow.DayOfYear - lastPostDate.DayOfYear) <= 1)
                {
                    context?.Succeed(requirement);
                    return;
                }
                var authorizationFailureReason = new AuthorizationFailureReason(this, "Posts Are Forbidden.");
                context?.Fail(authorizationFailureReason);
                return;
            }
        }
        else
        {
            var authorizationFailureReason = new AuthorizationFailureReason(this, "Error with user id.");
            context?.Fail(authorizationFailureReason);
            return;
        }
    }
}
