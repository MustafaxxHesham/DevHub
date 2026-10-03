namespace DevHub.AuthorizationPolicies;

public static class AuthPolicies
{
    public const string AddPostPolicy = "Add Post";
    public const string AddCommentPolicy = "Add Comment";
}

/*
1. Platform Subscription
2. Author Subscription
3. (Both)

Table. SubscriptionPlan     (Id, Name, Price, BillingCycle, Description, CreatedAt)  <Data> -->(Free, Pro, Premium)
Table. UserSubscription     (Id, UserId, PlanId, StartDate, EndDate, IsActive, PaymentProvider, PaymentId)

Modify Posts Table Add Column (AccessLevel) [Free, Premium, SubscribersOnly]

*/