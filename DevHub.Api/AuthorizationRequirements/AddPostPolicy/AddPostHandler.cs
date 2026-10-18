using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Enums;
using DevHub.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace DevHub.AuthorizationRequirements.AddPostPolicy;
public class AddPostHandler(IDataStore _dataStore,
                            ProtectionHandler _protectionHandler) : 
                            AuthorizationHandler<AddPostRequirement>, IAuthorizationHandler
{
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

        int userId = Common.RetrieveUserId(context.User, _protectionHandler);

        if (userId > 0)
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
