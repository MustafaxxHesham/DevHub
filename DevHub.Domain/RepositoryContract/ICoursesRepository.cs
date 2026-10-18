using DevHub.Domain.Helpers;
using DevHub.Domain.LogicContract;
using DevHub.Domain.Models;

namespace DevHub.Domain.RepositoryContract;
public interface ICoursesRepository : IBaseRepository<Course, int>
{
    Task<Course> GetCourseById(int id);
    Task<IPagedList<Tuple<Course, double>>> GetEnrolledCoursesAsync(int pageSize, int pageNumber, int userId, int? lastId);
    Task<IPagedList<Course>> GetBestsellersAsync(int pageSize, int pageNumber, int? lastId);
    IQueryable<Course> GetCoursesBySearch(string query);
    Task<bool> IsUserCourseInstructor(int courseId, int userId);
}