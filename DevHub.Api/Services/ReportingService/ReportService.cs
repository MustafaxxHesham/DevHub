using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Enums;
using DevHub.Domain.Helpers;
using DevHub.Domain.Models;
using DevHub.Domain.Result;
using DevHub.DTOS.Report;
using DevHub.EFCore.ErrorTypes;
using DevHub.Responses;
using DevHub.Utilities;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;

namespace DevHub.Services.ReportingService;

public class ReportService(IDataProtectionProvider provider, IDataStore _dataStore) : IReportService
{
    private readonly Dictionary<string, IDataProtector> _protectors = new()
    {
        ["userId"] = provider.CreateProtector(ProtectionPurposes.USER_ID_PURPOSE),
        ["commentId"] = provider.CreateProtector(ProtectionPurposes.COMMENT_ID_PURPOSE),
        ["postId"] = provider.CreateProtector(ProtectionPurposes.POST_ID_PURPOSE),
        ["reportId"] = provider.CreateProtector(ProtectionPurposes.REPORT_ID_PURPOSE)
    };

    public async Task<Result<ReportResponse>> GetReportAsync(string reportId)
    {
        var reportIdInInt = getIntId(reportId, ProtectionPurposes.REPORT_ID_PURPOSE);

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

        var reportsResponse = convertToReportListResponse(reportsByMonth.ValueList);

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

        var values = convertToReportListResponse(reports.ValueList);

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

        var values = convertToReportListResponse(reports.ValueList);

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

        var values = convertToReportListResponse(reports.ValueList);

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

        var values = convertToReportListResponse(reports.ValueList);

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

        var values = convertToReportListResponse(reports.ValueList);

        var response = PagedResponse<ReportResponse>.Create(values, reports.PageSize, reports.CurrentPage, reports.TotalPages);

        return Result<PagedResponse<ReportResponse>>.Success(response);
    }
    public async Task<Result<PagedResponse<ReportResponse>>> GetReportsByUserAsync(KeyPagedRequest<string> request)
    {
        int cId = 0;
        try
        {
            cId = getIntId(request.Key, ProtectionPurposes.USER_ID_PURPOSE);
        }
        catch (Exception ex)
        {
            return Result<PagedResponse<ReportResponse>>.Failure(ex.Message);
        }

        if (!await _dataStore.Comments.IsExistAsync(cId))
        {
            return Result<PagedResponse<ReportResponse>>.Failure(DbErrors.NotFoundError.ToString() + nameof(Comment));
        }

        var reports = await _dataStore.Reports.GetPaginatedByCriteriaAsync(request.PageSize, request.PageNumber, x => x.ReporterId == cId, x => x.Id);

        if (!reports.ValueList.Any())
        {
            return Result<PagedResponse<ReportResponse>>.Failure(DbErrors.NotFoundError.ToString());
        }

        var values = convertToReportListResponse(reports.ValueList);

        var response = PagedResponse<ReportResponse>.Create(values, reports.PageSize, reports.CurrentPage, reports.TotalPages);

        return Result<PagedResponse<ReportResponse>>.Success(response);

    }
    public async Task<Result<ReportAnswerResponse>> GetAnswerReportAsync(string reportId)
    {
        var realReportId = getIntId(reportId, "reportId");

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
            ReporterId:_protectors["userId"].Protect(reportAnswer.ReporterId.ToString()),
            ReportId:_protectors["reportId"].Protect(reportAnswer.ReportId.ToString())
        );

        return Result<ReportAnswerResponse>.Success(reportAnswerResponse);
    }
    public async Task<SimpleResult<bool>> DeleteReportAsync(string reportId)
    {
        var reportIdInInt = getIntId(reportId, "reportId");

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
            rId = getIntId(reportId, ProtectionPurposes.REPORT_ID_PURPOSE);
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
            userId = getIntId(request.UserId, ProtectionPurposes.USER_ID_PURPOSE);
            reporterId = getIntId(request.ReporterId, ProtectionPurposes.USER_ID_PURPOSE);
            postId = getIntId(request.PostId, ProtectionPurposes.POST_ID_PURPOSE);
            if (!string.IsNullOrEmpty(request.CommentId))
            {
                commentId = getIntId(request.CommentId, ProtectionPurposes.COMMENT_ID_PURPOSE);
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

                report.CommentId = getIntId(request.CommentId, ProtectionPurposes.COMMENT_ID_PURPOSE);
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
        int reporterId = getIntId(request.ReporterId, "userId");

        if (reporterId == -1)
        {
            return SimpleResult<int>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }
        else if (await _dataStore.Users.IsExistAsync(reporterId))
        {
            return SimpleResult<int>.Failure(ResponseMessages.USER_NOT_FOUND);
        }

        int reportId = getIntId(request.ReportId, "reportId");

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
            ReportId: _protectors[ProtectionPurposes.REPORT_ID_PURPOSE].Protect(report.Id.ToString()),
            IsAdminViewed: report.IsAdminViewed,
            CreatedAt: report.CreatedAt,
            ReporterName: report.Reporter.FirstName + " " + report.Reporter.LastName,
            Type: report.Type,
            UserName: report.User.FirstName + " " + report.User.LastName,
            ReporterId: _protectors[ProtectionPurposes.USER_ID_PURPOSE].Protect(report.ReporterId.ToString()),
            UserId: _protectors[ProtectionPurposes.USER_ID_PURPOSE].Protect(report.UserId.ToString()),
            Details: report.ReportDetails
        );
        return reportResponse;
    }
    private List<ReportResponse> convertToReportListResponse(IEnumerable<Report> reports)
    {
        List<ReportResponse> result = new List<ReportResponse>();
        foreach (var report in reports)
        {
            var reportResponse = new ReportResponse(
                ReportId: _protectors["reportId"].Protect(report.Id.ToString()),
                IsAdminViewed: report.IsAdminViewed,
                CreatedAt: report.CreatedAt,
                ReporterName: report.Reporter.FirstName + " " + report.Reporter.LastName,
                Type: report.Type,
                UserName: report.User.FirstName + " " + report.User.LastName,
                ReporterId: _protectors["userId"].Protect(report.ReporterId.ToString()),
                UserId: _protectors["userId"].Protect(report.UserId.ToString()),
                Details: report.ReportDetails
            );
            result.Add(reportResponse);
        }
        return result;
    }
    private int getIntId(string id, string key)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                return -1;
            }

            int intId = int.Parse(_protectors[key].Unprotect(id));

            return intId;
        }


        catch (Exception ex)
        {
            return ex switch
            {
                FormatException => -2,
                ArgumentException => -3,
                OverflowException => -4,
                CryptographicException => -5,
            };
        }
    }
}