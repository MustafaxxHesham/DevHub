using System.Linq.Expressions;
namespace DevHub.Services.BackgroundService;
public interface IBackgroundService
{
    string AddBackgroundJob(Expression<Action> method);
    void RemoveBackgroundJob(string jobId);
}