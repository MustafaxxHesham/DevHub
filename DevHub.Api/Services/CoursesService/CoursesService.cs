using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Models;
using DevHub.Domain.Result;
using DevHub.DTOS.Commons;
using DevHub.DTOS.CourseChapters;
using DevHub.DTOS.Courses;
using DevHub.Responses;
using DevHub.Utilities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Services.CoursesService;

enum Keys
{
    InstructorId,
    CategoryId,
    CourseId,
    CourseChapterId,
    CourseVideoId
}
public class CoursesService(IDataStore _dataStore, IDataProtectionProvider _provider) : ICoursesService
{
    private readonly Dictionary<string, IDataProtector> _protectors = new()
    {
        [Keys.CourseId.ToString()]=_provider.CreateProtector(Keys.CourseId.ToString()),
        [Keys.CourseChapterId.ToString()] =_provider.CreateProtector(Keys.CourseChapterId.ToString()),
        [Keys.CourseVideoId.ToString()] =_provider.CreateProtector(Keys.CourseVideoId.ToString()),
        [Keys.CategoryId.ToString()] =_provider.CreateProtector("categoryId"),
        [Keys.InstructorId.ToString()] =_provider.CreateProtector("userId"),
    };
 
    
    public async Task<Result<CourseResponse>> GetCourseByIdAsync(string courseId)
    {
        var realCourseId = getRealInt(courseId, Keys.CourseId);
        
        if (realCourseId == -1 && false)
        {
            return Result<CourseResponse>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }

        var course = await _dataStore.Courses.GetCourseById(realCourseId)
            .Select(c => new CourseResponse
            (
                CourseId: _protectors[Keys.CourseId.ToString()].Protect(c.Id.ToString()),
                Title: c.Title,
                Description: c.Description,
                CategoryName: c.Category.Name,
                CategoryId: _protectors[Keys.CategoryId.ToString()].Protect(c.CategoryId.ToString()),
                CourseImageUrl: c.CourseImageUrl,
                Price: c.Price,
                InstructorId: _protectors[Keys.InstructorId.ToString()].Protect(c.InstructorId.ToString())
            )).FirstOrDefaultAsync();


        if (course == null)
        {
            return Result<CourseResponse>.Failure(ResponseMessages.COURSE_NOT_FOUND);
        }

        return Result<CourseResponse>.Success(course);
    }

    public Task<PagedResponse<CourseResponse>> GetCoursesByCategoryAsync()
    {
        throw new NotImplementedException();
    }

    public Task<PagedResponse<CourseResponse>> GetCoursesByInstructorAsync()
    {
        throw new NotImplementedException();
    }

    public Task<PagedResponse<CourseResponse>> GetCoursesFreeAsync()
    {
        throw new NotImplementedException();
    }

    public Task<PagedResponse<CourseResponse>> GetCoursesProAsync()
    {
        throw new NotImplementedException();
    }


    public async Task<PagedResponse<CourseResponse>> SearchCoursesAsync(PagedSearchRequest request)
    {
        var coursesList = await _dataStore.Courses.GetPaginatedByCriteriaAsync(request.pageSize, 
            request.pageNumber, x => x.Title.Contains(request.searchKey) 
        || x.Description.Contains(request.searchKey), x => x.Id, ["Instructor", "Category"]);

        var result = getHashedCoursesList(coursesList.ValueList);

        return PagedResponse<CourseResponse>.Create(result, coursesList.PageSize, coursesList.CurrentPage, coursesList.TotalPages);
    }

    private List<CourseResponse> getHashedCoursesList(List<Course> courses)
    {
        List<CourseResponse> courseResponses = new();
        foreach (var course in courses) {
            courseResponses.Add(getHashedCourse(course));
        }
        return courseResponses;
    }
    private CourseResponse getHashedCourse(Course course)
    {
        return new CourseResponse(
                CourseId:_protectors[Keys.CourseId.ToString()].Protect(course.Id.ToString()),
                Title: course.Title,
                Description: course.Description,
                CategoryName: course.Category.Name,
                CategoryId: _protectors[Keys.CategoryId.ToString()].Protect(course.CategoryId.ToString()),
                CourseImageUrl: course.CourseImageUrl,
                Price: course.Price,
                InstructorId: _protectors[Keys.InstructorId.ToString()].Protect(course.InstructorId.ToString())
            );
    }
    private int getRealInt(string itemId, Keys key) 
    {
        try
        {
            return int.Parse(_protectors[key.ToString()].Unprotect(itemId));

        } catch (Exception ex)
        {
            return -1;
        }
    }

    public Task<PagedResponse<CourseChapterResponse>> GetChaptersAsync(CourseChapterRequest request)
    {
        throw new NotImplementedException();
    }
}
