using DevHub.Domain.Enums;

namespace DevHub.DTOS.Report;

public record class SaveReportRequest(string ReportDetails, 
    ReportType Type, 
    string ReporterId, 
    string UserId,
    string PostId,
    string? CommentId
);