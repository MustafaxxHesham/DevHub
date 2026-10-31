namespace DevHub.Utilities;
public interface INotificationActions
{
    Task NotifyAuthorByComment(object request);
    Task NotifyClientByReportResponse();
    Task NotifyAdminByReportRequests();
}