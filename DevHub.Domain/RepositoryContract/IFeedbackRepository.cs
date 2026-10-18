using DevHub.Domain.Enums;
using DevHub.Domain.Helpers;
using DevHub.Domain.LogicContract;
using DevHub.Domain.Models;

namespace DevHub.Domain.RepositoryContract;

public interface IFeedbackRepository : IBaseRepository<Feedback, int>
{
    Task<IPagedList<Feedback>> GetPagedFeedbacksAsync(int pageSize, int pageNumber, int courseId, int? lastId,
        Sorting sorting = Sorting.Ascending);
    Task<IPagedList<Feedback>> GetPagedFeedbacksByRateAsync(int rate, int pageSize, int pageNumber, int courseId, int? lastId,
        Sorting sorting = Sorting.Ascending);
    Task<Dictionary<string, int>> GetRateCountedAsync(int courseId);
}
