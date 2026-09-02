namespace DevHub.AuthorizationClaims;

public class AuthorizationSchemas
{
    //
}
/*
1. Platform Subscription
2. Author Subscription
3. (Both)

Table. SubscriptionPlan     (Id, Name, Price, BillingCycle, Description, CreatedAt)  <Data> -->(Free, Pro, Premium)
Table. UserSubscription     (Id, UserId, PlanId, StartDate, EndDate, IsActive, PaymentProvider, PaymentId)
Table. AuthorSubscription   (Id, UserId, AuthorId, Price, StartDate, EndDate, IsActive)

Modify Posts Table Add Column (AccessLevel) [Free, Premium, SubscribersOnly]

*/