using DevHub.Domain.DataStoreContract;
using DevHub.DTOS.Common;
using DevHub.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace DevHub.AuthorizationRequirements.EditCoursePolicy;

public class EditCourseHandler(IDataStore _dataStore, ProtectionHandler _protectionHandler) : AuthorizationHandler<EditCourseRequirement>, IAuthorizationHandler
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, EditCourseRequirement requirement)
    {
        var courseOwner = context.Resource as CourseOwner;

        int realUserId = Common.RetrieveUserId(context.User, _protectionHandler);

        if (realUserId == -1)
        {
            context.Fail();
            return;
        }

        int realCourseId = _protectionHandler.GetRealCourseId(courseOwner!.CourseId);

        var isUserCourseInstructor = await _dataStore.Courses.IsUserCourseInstructor(realCourseId, realUserId);

        if (isUserCourseInstructor || await _dataStore.Users.IsUserAdmin(realUserId))
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail();
        return;
    }
}
