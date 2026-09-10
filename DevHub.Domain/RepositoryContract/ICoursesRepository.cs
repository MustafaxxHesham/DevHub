using DevHub.Domain.LogicContract;
using DevHub.Domain.Models;

namespace DevHub.Domain.RepositoryContract;
public interface ICoursesRepository : IBaseRepository<Course, int>
{
    IQueryable<Course> GetCourseById(int id);
    IQueryable<Course> GetCoursesBySearch(string query);
}