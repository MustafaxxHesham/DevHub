using DevHub.Domain.Enums;
namespace DevHub.DTOS.Report;
public record class PagedReportMonthlyRequest(int Month, int PageSize, int PageNumber, ReportType? ReportType);