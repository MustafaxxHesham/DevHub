using Hangfire;
using System.Linq.Expressions;

namespace DevHub.Services.BackgroundService;

public class BackgroundService(RecommendationService.RecommendationService _recommendationService) : IBackgroundService 
{
    public string AddBackgroundJob(Expression<Action> method)
    {
        string userSubscriptionId = Guid.NewGuid().ToString();
        RecurringJob.AddOrUpdate(userSubscriptionId, method, Cron.Monthly());
        return userSubscriptionId;
    }

    public void RemoveBackgroundJob(string jobId)
    {
        RecurringJob.RemoveIfExists(jobId);
    }

    public void UpdatePostsViewsCountHourly(int hourScheduled)
    {
        BackgroundJob.Schedule(() => _recommendationService.BulkUpdateForPostViews(), TimeSpan.FromHours(hourScheduled));
    }

}
