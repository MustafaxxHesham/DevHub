using DevHub.Domain.Enums;

namespace DevHub.DTOS.Report;

public record class ReportResponse(
    string ReportId,
    bool IsAdminViewed,
    ReportType Type,
    string ReporterId,
    string UserId,
    string ReporterName,
    string UserName,
    DateTime CreatedAt,
    string Details
);
