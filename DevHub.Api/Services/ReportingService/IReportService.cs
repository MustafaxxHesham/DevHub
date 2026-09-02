using DevHub.Domain.Result;
using DevHub.DTOS.Report;
using DevHub.Utilities;

namespace DevHub.Services.ReportingService;
public interface IReportService
{
    Task<Result<ReportResponse>> GetReportAsync(string reportId);
    Task<Result<PagedResponse<ReportResponse>>> GetReportsAsync(int pageSize, int pageNumber);
    Task<Result<PagedResponse<ReportResponse>>> GetReportsByAdminViewAsync(bool adminViewed, int pageSize, int pageNumber);
    Task<Result<PagedResponse<ReportResponse>>> GetReportsByUserAsync(PagedRequest request);
    Task<Result<PagedResponse<ReportResponse>>> GetReportsForCommentsAsync(int pageSize, int pageNumber);
    Task<Result<PagedResponse<ReportResponse>>> GetReportsForPostsAsync(int pageSize, int pageNumber);
    Task<Result<PagedResponse<ReportResponse>>> GetMonthlyReportsAsync(PagedReportMonthlyRequest request);
    Task<Result<PagedResponse<ReportResponse>>> GetWeeklyReportsAsync(int week, int pageSize, int pageNumber);
    Task<SimpleResult<bool>> SaveReportAsync(SaveReportRequest request);
    Task<SimpleResult<bool>> MarkReportAsViewedAsync(string reportId);
    Task<SimpleResult<bool>> DeleteReportAsync(string reportId);
}