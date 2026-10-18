using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Enums;
using DevHub.Domain.Models;
using DevHub.Domain.Result;
using DevHub.DTOS.Report;
using DevHub.EFCore.ErrorTypes;
using DevHub.Responses;
using DevHub.Utilities;
using System.Security.Cryptography;

namespace DevHub.Services.ReportingService;

public class ReportService(ProtectionHandler _protectionHandler, IDataStore _dataStore) : IReportService
{
    public async Task<Result<ReportResponse>> GetReportAsync(string reportId)
    {
        var reportIdInInt = _protectionHandler.GetRealReportId(reportId);

        if (reportIdInInt < 0)
        {
            return reportIdInInt switch
            {
                -1 => Result<ReportResponse>.Failure("Id not found!"),
                -2 => Result<ReportResponse>.Failure("Problem about id"),
                -3 => Result<ReportResponse>.Failure("Problem about id"),
                -4 => Result<ReportResponse>.Failure("Problem about id"),
                _ => Result<ReportResponse>.Failure("Problem with id")
            };
        }

        var report = await _dataStore.Reports.GetByCriteriaFirstAsync(r => r.Id == reportIdInInt, ["Reporter", "User"]);

        if (report == null)
        {
            return Result<ReportResponse>.Failure(DbErrors.NotFoundError.ToString());
        }

        var ReportResponse = convertToReportResponse(report);

        return Result<ReportResponse>.Success(ReportResponse);
    }
    public async Task<Result<PagedResponse<ReportResponse>>> GetMonthlyReportsAsync(PagedReportMonthlyRequest request)
    {

        var reportsByMonth = await _dataStore.Reports.GetByCriteriaAsync(x => x.CreatedAt.Month == request.Month && (request.ReportType.HasValue) ? request.ReportType.Value == x.Type : true, x => x.Id, request.PageSize, request.PageNumber);

        if (!reportsByMonth.ValueList.Any())
        {
            return Result<PagedResponse<ReportResponse>>.Failure(DbErrors.NotFoundError.ToString());
        }

        var reportsResponse = ConvertToReportListResponse(reportsByMonth.ValueList);

        var pagedResponse = PagedResponse<ReportResponse>.Create(reportsResponse, reportsByMonth.PageSize, reportsByMonth.CurrentPage, reportsByMonth.TotalPages);

        return Result<PagedResponse<ReportResponse>>.Success(pagedResponse);
    }
    public async Task<Result<PagedResponse<ReportResponse>>> GetReportsAsync(int pageSize, int pageNumber)
    {
        var reports = await _dataStore.Reports.GetPaginatedAsync(pageSize, pageNumber, x => x.Id);

        if (!reports.ValueList.Any())
        {
            return Result<PagedResponse<ReportResponse>>.Failure(DbErrors.NotFoundError.ToString());
        }

        var values = ConvertToReportListResponse(reports.ValueList);

        var response = PagedResponse<ReportResponse>.Create(values, reports.PageSize, reports.CurrentPage, reports.TotalPages);

        return Result<PagedResponse<ReportResponse>>.Success(response);
    }
    public async Task<Result<PagedResponse<ReportResponse>>> GetReportsByAdminViewAsync(bool adminViewed, int pageSize, int pageNumber)
    {
        var reports = await _dataStore.Reports.GetPaginatedByCriteriaAsync(pageSize, pageNumber, x => x.IsAdminViewed == adminViewed, x => x.Id);

        if (!reports.ValueList.Any())
        {
            return Result<PagedResponse<ReportResponse>>.Failure(DbErrors.NotFoundError.ToString());
        }

        var values = ConvertToReportListResponse(reports.ValueList);

        var pagedResponse = PagedResponse<ReportResponse>.Create(values, reports.PageSize, reports.CurrentPage, reports.TotalPages);

        return Result<PagedResponse<ReportResponse>>.Success(pagedResponse);
    }
    public async Task<Result<PagedResponse<ReportResponse>>> GetReportsForCommentsAsync(int pageSize, int pageNumber)
    {
        var reports = await _dataStore.Reports.GetPaginatedByCriteriaAsync(pageSize, pageNumber, x => x.CommentId != null, x => x.Id);

        if (!reports.ValueList.Any())
        {
            return Result<PagedResponse<ReportResponse>>.Failure(DbErrors.NotFoundError.ToString());
        }

        var values = ConvertToReportListResponse(reports.ValueList);

        var response = PagedResponse<ReportResponse>.Create(values, reports.PageSize, reports.CurrentPage, reports.TotalPages);

        return Result<PagedResponse<ReportResponse>>.Success(response);
    }
    public async Task<Result<PagedResponse<ReportResponse>>> GetReportsForPostsAsync(int pageSize, int pageNumber)
    {
        var reports = await _dataStore.Reports.GetByCriteriaAsync(x => x.PostId != null, x => x.Id, pageSize, pageNumber);

        if (!reports.ValueList.Any())
        {
            return Result<PagedResponse<ReportResponse>>.Failure(DbErrors.NotFoundError.ToString());
        }

        var values = ConvertToReportListResponse(reports.ValueList);

        var response = PagedResponse<ReportResponse>.Create(values, reports.PageSize, reports.CurrentPage, reports.TotalPages);

        return Result<PagedResponse<ReportResponse>>.Success(response);
    }
    public async Task<Result<PagedResponse<ReportResponse>>> GetWeeklyReportsAsync(KeyPagedRequest<int> request)
    {
        // needs logic to paginatiion
        var reports = await _dataStore.Reports.GetByCriteriaAsync(x => x.CreatedAt < (DateTime.Today.AddDays(-7 * request.Key)), x => x.Id, request.PageSize, request.PageNumber,
            ["Reporter", "User"]);

        if (!reports.ValueList.Any())
        {
            return Result<PagedResponse<ReportResponse>>.Failure(DbErrors.NotFoundError.ToString());
        }

        var values = ConvertToReportListResponse(reports.ValueList);

        var response = PagedResponse<ReportResponse>.Create(values, reports.PageSize, reports.CurrentPage, reports.TotalPages);

        return Result<PagedResponse<ReportResponse>>.Success(response);
    }
    public async Task<Result<PagedResponse<ReportResponse>>> GetReportsByUserAsync(KeyPagedRequest<string> request)
    {
        int userId = _protectionHandler.GetRealUserId(request.Key);

        if (userId == -1)
        {
            // to be continued...
        }

        if (!await _dataStore.Comments.IsExistAsync(userId))
        {
            return Result<PagedResponse<ReportResponse>>.Failure(DbErrors.NotFoundError.ToString() + nameof(Comment));
        }

        var reports = await _dataStore.Reports.GetPaginatedByCriteriaAsync(request.PageSize, request.PageNumber, x => x.ReporterId == userId, x => x.Id);

        if (!reports.ValueList.Any())
        {
            return Result<PagedResponse<ReportResponse>>.Failure(DbErrors.NotFoundError.ToString());
        }

        var values = ConvertToReportListResponse(reports.ValueList);

        var response = PagedResponse<ReportResponse>.Create(values, reports.PageSize, reports.CurrentPage, reports.TotalPages);

        return Result<PagedResponse<ReportResponse>>.Success(response);

    }
    public async Task<Result<ReportAnswerResponse>> GetAnswerReportAsync(string reportId)
    {
        var realReportId = _protectionHandler.GetRealReportId(reportId);

        if (realReportId == -1)
        {
            return Result<ReportAnswerResponse>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }

        var reportAnswer = await _dataStore.ReportAnswers.GetByIdAsync(realReportId);

        if (reportAnswer == null)
        {
            return Result<ReportAnswerResponse>.Failure(ResponseMessages.REPORT_ANSWER_NOT_FOUND);
        }

        var reportAnswerResponse = new ReportAnswerResponse
        (
            AnswerDetails:reportAnswer.ReportDetails,
            ReporterId: _protectionHandler.GetProtectedUserId(reportAnswer.ReporterId),
            ReportId: _protectionHandler.GetProtectedReportId(reportAnswer.ReportId)
        );

        return Result<ReportAnswerResponse>.Success(reportAnswerResponse);
    }
    public async Task<SimpleResult<bool>> DeleteReportAsync(string reportId)
    {
        var reportIdInInt = _protectionHandler.GetRealReportId(reportId);
        // to be checked
        if (reportIdInInt < 0)
        {
            return reportIdInInt switch
            {
                -1 => SimpleResult<bool>.Failure("Id not found!"),
                -2 => SimpleResult<bool>.Failure("Problem about id"),
                -3 => SimpleResult<bool>.Failure("Problem about id"),
                -4 => SimpleResult<bool>.Failure("Problem about id"),
                _ => SimpleResult<bool>.Failure("Problem with id")
            };
        }

        var report = await _dataStore.Reports.GetByIdAsync(reportIdInInt);

        if (report is null)
        {
            SimpleResult<bool>.Failure(DbErrors.NotFoundError.ToString());
        }

        _dataStore.Reports.RemoveItem(report);

        await _dataStore.CompleteAsync();

        return SimpleResult<bool>.Success(true);
    }
    public async Task<SimpleResult<bool>> MarkReportAsViewedAsync(string reportId)
    {
        int rId = 0;
        try
        {
            rId = _protectionHandler.GetRealReportId(reportId);
        }
        catch (Exception ex) {

        }

        var report = await _dataStore.Reports.GetByIdAsync(rId);

        if (report is null)
        {
            return SimpleResult<bool>.Failure(DbErrors.NotFoundError.ToString());
        }

        if (report.IsAdminViewed)
        {
            return SimpleResult<bool>.Failure("Item is already viewed");
        }

        report.IsAdminViewed = true;

        _dataStore.Reports.UpdateItem(report);

        await _dataStore.CompleteAsync();

        return SimpleResult<bool>.Success(true);

    }
    public async Task<SimpleResult<bool>> SaveReportAsync(SaveReportRequest request)
    {
        int userId = 0;
        int reporterId = 0;
        int postId = 0;
        int commentId = 0;
        try
        {
            userId = _protectionHandler.GetRealUserId(request.UserId);
            reporterId = _protectionHandler.GetRealUserId(request.ReporterId);
            postId = _protectionHandler.GetRealPostId(request.PostId);
            if (!string.IsNullOrEmpty(request.CommentId))
            {
                commentId = _protectionHandler.GetRealCommentId(request.CommentId);
            }
        }
        catch (Exception ex)
        {
            return ex switch
            {
                CryptographicException => SimpleResult<bool>.Failure("Error due to manipulating with cryptographic exception."),
                OverflowException => SimpleResult<bool>.Failure("Error due to manipulating with cryptographic exception."),
            };
        }

        if (!await _dataStore.Users.IsExistAsync(userId))
        {
            return SimpleResult<bool>.Failure("User not found!");
        }

        if (!await _dataStore.Users.IsExistAsync(reporterId))
        {
            return SimpleResult<bool>.Failure("User not found!");
        }

        if (!await _dataStore.Posts.IsExistAsync(postId))
        {
            return SimpleResult<bool>.Failure("Post not found!");
        }
        var report = new Report
        {
            Type = request.Type,
            ReportDetails = request.ReportDetails,
            UserId = userId,
            ReporterId = reporterId,
            PostId = reporterId,
        };

        if (report.Type == ReportType.REPORT_COMMENT)
        {
            if (string.IsNullOrEmpty(request.CommentId))
            {

                report.CommentId = _protectionHandler.GetRealCommentId(request.CommentId);
                if (report.CommentId.Value <= 0)
                {
                    return report.CommentId switch
                    {
                        -1 => SimpleResult<bool>.Failure("Id not found!"),
                        -2 => SimpleResult<bool>.Failure("Problem about id"),
                        -3 => SimpleResult<bool>.Failure("Problem about id"),
                        -4 => SimpleResult<bool>.Failure("Problem about id"),
                        _ => SimpleResult<bool>.Failure("Problem with id")

                    };
                }
            }
            else
            {
                return SimpleResult<bool>.Failure("Missing comment Id");
            }
        }

        await _dataStore.Reports.AddAsync(report);

        await _dataStore.CompleteAsync();

        return SimpleResult<bool>.Success(true);

    }
    public async Task<SimpleResult<int>> AnswerReportAsync(ReportAnswerRequest request)
    {
        int reporterId = _protectionHandler.GetRealUserId(request.ReporterId);

        if (reporterId == -1)
        {
            return SimpleResult<int>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }
        else if (await _dataStore.Users.IsExistAsync(reporterId))
        {
            return SimpleResult<int>.Failure(ResponseMessages.USER_NOT_FOUND);
        }

        int reportId = _protectionHandler.GetRealReportId(request.ReportId);

        if (reporterId == -1)
        {
            return SimpleResult<int>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }

        else if (await _dataStore.Reports.IsExistAsync(reportId))
        {
            return SimpleResult<int>.Failure(ResponseMessages.REPORT_NOT_FOUND);
        }

        if (await _dataStore.ReportAnswers.IsExistAsync(reportId))
        {
            return SimpleResult<int>.Failure(ResponseMessages.ACTION_ALREADY_DONE);
        }

        var reportAnswer = new ReportAnswer
        {
            IsViewed = false,
            ReportDetails = request.AnswerDetails,
            CreatedAt = DateTime.UtcNow,
            ReporterId = reporterId,
            ReportId = reportId
        };

        await _dataStore.ReportAnswers.AddAsync(reportAnswer);
        await _dataStore.CompleteAsync();

        return SimpleResult<int>.Success(reportId);

    }



    private ReportResponse convertToReportResponse(Report report)
    {
        var reportResponse = new ReportResponse(
            ReportId: _protectionHandler.GetProtectedReportId(report.Id),
            IsAdminViewed: report.IsAdminViewed,
            CreatedAt: report.CreatedAt,
            ReporterName: report.Reporter.FirstName + " " + report.Reporter.LastName,
            Type: report.Type,
            UserName: report.User.FirstName + " " + report.User.LastName,
            ReporterId: _protectionHandler.GetProtectedUserId(report.ReporterId),
            UserId: _protectionHandler.GetProtectedUserId(report.UserId),
            Details: report.ReportDetails
        );
        return reportResponse;
    }
    private List<ReportResponse> ConvertToReportListResponse(IEnumerable<Report> reports)
    {
        List<ReportResponse> result = new List<ReportResponse>();
        foreach (var report in reports)
        {
            var reportResponse = new ReportResponse(
                ReportId: _protectionHandler.GetProtectedReportId(report.Id),
                IsAdminViewed: report.IsAdminViewed,
                CreatedAt: report.CreatedAt,
                ReporterName: report.Reporter.FirstName + " " + report.Reporter.LastName,
                Type: report.Type,
                UserName: report.User.FirstName + " " + report.User.LastName,
                ReporterId: _protectionHandler.GetProtectedUserId(report.ReporterId),
                UserId: _protectionHandler.GetProtectedUserId(report.UserId),
                Details: report.ReportDetails
            );
            result.Add(reportResponse);
        }
        return result;
    }
}