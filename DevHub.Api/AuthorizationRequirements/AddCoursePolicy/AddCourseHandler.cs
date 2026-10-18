using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Enums;
using DevHub.DTOS.Courses;
using DevHub.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace DevHub.AuthorizationRequirements.AddCoursePolicy;

public class AddCourseHandler(ProtectionHandler _protectionHandler, IDataStore _dataStore) : AuthorizationHandler<AddCourseRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AddCourseRequirement requirement)
    {
        var request = context.Resource as CreateCourseRequest;

        var currentUserId = Common.RetrieveUserId(context.User, _protectionHandler);

        if (currentUserId == -1)
        {
            return;
        }

        var isAccountActive = await _dataStore.Users.IsExistWithCriteriaAsync(u => u.IsActive && u.Id == currentUserId);

        if (!isAccountActive)
        {
            context.Fail();
            return;
        }

        var currentPlan = await _dataStore.Users.GetUserSubscriptionPlan(currentUserId);

        if (currentPlan == SubscriptionPlans.Free)
        {
            if(request?.Price == 0)
            {
                context.Succeed(requirement);
                return;
            }
            context.Fail();
            return;
        }

        context.Succeed(requirement);
        return;   
    }
}