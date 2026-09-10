using Data.Layer.EFCore;
using Data.Layer.EFCore.Repository;
using DevHub.Domain.Models;
using DevHub.Domain.RepositoryContract;
using Microsoft.EntityFrameworkCore;

namespace DevHub.EFCore.Repository;

public class CoursesRepository(AppDbContext _context) : BaseRepository<Course, int>(_context), ICoursesRepository
{
    public IQueryable<Course> GetCourseById(int id)
    {
        return _context.Courses.Where(x => x.Id == id)
                               .Include(x => x.Instructor)
                               .Include(x => x.Category);
    }

    public IQueryable<Course> GetCoursesBySearch(string query)
    {
        return _context.Courses.Where(x => x.Title.Contains(query) || x.Description.Contains(query))
                               .Include(x => x.Instructor)
                               .Include(x => x.Category);
    }
}
