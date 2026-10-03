namespace DevHub.Utilities;
public interface INotificationComment
{
    Task NotifyAuthorByComment(object request);
    Task NotifyClientByReportResponse();
    Task NotifyAdminByReportRequests();
}