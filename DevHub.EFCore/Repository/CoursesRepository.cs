using Data.Layer.EFCore;
using Data.Layer.EFCore.Repository;
using DevHub.Domain.Helpers;
using DevHub.Domain.Models;
using DevHub.Domain.RepositoryContract;
using DevHub.EFCore.EFCorePaginationHelper;
using Microsoft.EntityFrameworkCore;

namespace DevHub.EFCore.Repository;

public class CoursesRepository(AppDbContext _context) : BaseRepository<Course, int>(_context), ICoursesRepository
{
    public async Task<Course> GetCourseById(int id)
    {
        return await _context.Courses.Where(x => x.Id == id)
                               .Include(x => x.Instructor)
                               .Include(x => x.Category)
                               .Select(c => new Course
                               {
                                   CourseImageUrl = c.CourseImageUrl,
                                   CategoryId = c.CategoryId,
                                   Category = c.Category,
                                   Description = c.Description,
                                   InstructorId = c.InstructorId,
                                   Instructor = new User
                                   {
                                       ProfileImageUrl = c.Instructor.ProfileImageUrl,
                                       FirstName = c.Instructor.FirstName,
                                       LastName = c.Instructor.LastName,
                                   }
                               }).FirstOrDefaultAsync();
        ;
    }

    public IQueryable<Course> GetCoursesBySearch(string query)
    {
        return _context.Courses.Where(x => x.Title.Contains(query) || x.Description.Contains(query))
                               .Include(x => x.Instructor)
                               .Include(x => x.Category);
    }



    public async Task<Dictionary<int, int>> GetCoursesAsync()
    {
        throw new NotImplementedException();
        //Bestsellers Courses
        //return await _context.UsersCourses.GroupBy(x => x.CourseId)
        //    .OrderByDescending(x => x.Count())
        //    .ToDictionaryAsync(x => x.Key, x => x.Count());
    }

    public async Task<bool> IsUserCourseInstructor(int courseId, int userId)
    {
        return await _context.Courses.AnyAsync(x => x.Id == courseId && x.InstructorId == userId);
    }

    public async Task<IPagedList<Course>> GetBestsellersAsync(int pageSize, int pageNumber, int? lastId)
    {
        var query = _context.Courses.Where(c => c.IsPublished && c.Price > 0);

        if (lastId.HasValue)
        {
            query = query.Where(c => c.Id < lastId.Value);
        }
        
        query = query.OrderByDescending(c => c.UsersEnrolled.Count)
                     .ThenByDescending(c => c.Id)
                     .Skip((pageNumber - 1) * pageSize)
                     .Take(pageSize)
                     .Select(c => new Course
                     {
                         CourseImageUrl = c.CourseImageUrl,
                         CategoryId = c.CategoryId,
                         Category = c.Category,
                         Description = c.Description,
                         InstructorId = c.InstructorId,
                         Instructor = new User
                         {
                            ProfileImageUrl = c.Instructor.ProfileImageUrl,
                            FirstName = c.Instructor.FirstName,
                            LastName = c.Instructor.LastName,
                         }
                     });

        return await PagedResponse<Course>.CreateAsync(query, pageSize, pageNumber);
    }

    public async Task<IPagedList<Tuple<Course, double>>> GetEnrolledCoursesAsync(int pageSize, int pageNumber, int userId, int? lastId)
    {
        var coursesQuery = _context.UserCourses.Include(uc => uc.Course).Where(uc => uc.UserId == userId);

        if (lastId.HasValue)
        {
            coursesQuery = coursesQuery.Where(c => lastId > c.Id);
        }
        
        var newQuery =  coursesQuery.OrderBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new Tuple<Course, double>(x.Course, (double)(x.UserCourseTrackers.Count() > 0 ?
            x.UserCourseTrackers.Count() : 1 / x.Course.CourseChapters.Count) * 100));

        return await PagedResponse<Tuple<Course, double>>.CreateAsync(newQuery, pageSize, pageNumber);
    }
}
