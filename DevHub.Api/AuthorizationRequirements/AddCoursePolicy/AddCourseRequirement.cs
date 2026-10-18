using DevHub.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
namespace DevHub.AuthorizationRequirements.AddCoursePolicy;
public class AddCourseRequirement : IAuthorizationRequirement
{
    public List<SubscriptionPlans> AllowedPlansForPricedCourses = new List<SubscriptionPlans>
    {
        SubscriptionPlans.Preimum,
        SubscriptionPlans.Pro
    };
}