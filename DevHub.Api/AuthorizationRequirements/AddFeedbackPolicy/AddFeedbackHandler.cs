using DevHub.Domain.DataStoreContract;
using DevHub.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace DevHub.AuthorizationRequirements.AddFeedbackPolicy;

public class AddFeedbackHandler(ProtectionHandler _protectionHandler, IDataStore _dataStore) : AuthorizationHandler<AddFeedbackRequirement>, IAuthorizationHandler
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AddFeedbackRequirement requirement)
    {
        int userId = Common.RetrieveUserId(context?.User!, _protectionHandler);

        if (userId <= 0)
        {
            context!.Fail();
            return;
        }

        var httpContext = context.Resource as HttpContext;

        string protectedCourseId = httpContext.Request.RouteValues["courseId"] as string;

        if (protectedCourseId == null)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("Course Id is required", default(CancellationToken));
            return;
        }

        int courseId = _protectionHandler.GetRealCourseId(protectedCourseId);

        if (courseId <= 0)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("Invalid Course Id", default(CancellationToken));
            return;
        }

        var isUserEnrolled = await _dataStore.UserCourses.IsExistWithCriteriaAsync(x => x.CourseId == courseId && x.UserId == userId);

        if (!isUserEnrolled)
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }
}
