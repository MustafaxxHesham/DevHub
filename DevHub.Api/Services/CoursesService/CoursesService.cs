using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Enums;
using DevHub.Domain.Models;
using DevHub.Domain.Result;
using DevHub.DTOS.CourseChapters;
using DevHub.DTOS.Courses;
using DevHub.DTOS.CourseVideos;
using DevHub.Responses;
using DevHub.Utilities;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Services.CoursesService;

public class CoursesService(IDataStore _dataStore, ProtectionHandler _protectionHandler) : ICoursesService
{
    public async Task<Result<CourseResponse>> GetCourseByIdAsync(string courseId)
    {
        var realCourseId = _protectionHandler.GetRealCourseId(courseId);
        
        if (realCourseId == -1 && false)
        {
            return Result<CourseResponse>.Failure(ResponseMessages.COURSE_NOT_FOUND);
        }

        var course = await _dataStore.Courses.GetCourseById(realCourseId)
            .Select(c => new CourseResponse
            (
                CourseId: courseId,
                Title: c.Title,
                Description: c.Description,
                CategoryName: c.Category.Name,
                CategoryId: _protectionHandler.GetProtectedCategoryId(c.CategoryId),
                CourseImageUrl: c.CourseImageUrl,
                Price: c.Price,
                InstructorId: _protectionHandler.GetProtectedUserId(c.InstructorId)
            )).FirstOrDefaultAsync();


        if (course == null)
        {
            return Result<CourseResponse>.Failure(ResponseMessages.COURSE_NOT_FOUND);
        }

        return Result<CourseResponse>.Success(course);
    }   
    public async Task<Result<PagedResponse<CourseResponse>>> GetCoursesByCategoryAsync(KeyPagedRequest<string> request)
    {
        var realCategoryId = _protectionHandler.GetRealCategoryId(request.Key);

        if (realCategoryId == -1)
        {
            return Result<PagedResponse<CourseResponse>>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }

        if (await _dataStore.Categories.IsExistAsync(realCategoryId))
        {
            return Result<PagedResponse<CourseResponse>>.Failure(ResponseMessages.CATEGORY_NOT_FOUND);
        }

        var res = await _dataStore.Courses.GetPaginatedByCriteriaAsync(request.PageSize, request.PageNumber, x => x.CategoryId == realCategoryId, x => x.Id, ["Instructor", "Category"]);

        var result = getHashedCoursesList(res.ValueList);

        var data = PagedResponse<CourseResponse>.Create(result, res.PageSize, res.CurrentPage, res.TotalPages);

        return Result<PagedResponse<CourseResponse>>.Success(data);
    }
    public async Task<Result<PagedResponse<CourseResponse>>> GetCoursesByInstructorAsync(KeyPagedRequest<string> request)
    {
        var realInstructorId = _protectionHandler.GetRealUserId(request.Key);
        
        if (realInstructorId == -1)
        {
            return Result<PagedResponse<CourseResponse>>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }

        if (await _dataStore.Users.IsExistAsync(realInstructorId))
        {
            return Result<PagedResponse<CourseResponse>>.Failure(ResponseMessages.USER_NOT_FOUND);
        }

        var coursesList = await _dataStore.Courses.GetPaginatedByCriteriaAsync(request.PageSize, request.PageNumber, x => x.InstructorId == realInstructorId, x => x.Id, ["Instructor", "Category"]);

        var courseResponses = getHashedCoursesList(coursesList.ValueList);

        var data = PagedResponse<CourseResponse>.Create(courseResponses, coursesList.PageSize, coursesList.CurrentPage, coursesList.TotalPages);

        return Result<PagedResponse<CourseResponse>>.Success(data);
    }
    public async Task<PagedResponse<CourseResponse>> GetCoursesAsync()
    {
        throw new NotImplementedException();
    }
    public async Task<PagedResponse<CourseResponse>> GetCoursesProAsync()
    {
        throw new NotImplementedException();
    }
    public async Task<PagedResponse<CourseResponse>> SearchCoursesAsync(KeyPagedRequest<string> request)
    {
        var coursesList = await _dataStore.Courses.GetPaginatedByCriteriaAsync(request.PageSize, request.PageNumber,
            x => x.Title.Contains(request.Key) || x.Description.Contains(request.Key),
            x => new {x.Price, x.Id }, ["Instructor", "Category"], Sorting.Descending);

        var result = getHashedCoursesList(coursesList.ValueList);

        return PagedResponse<CourseResponse>.Create(result, coursesList.PageSize, coursesList.CurrentPage, coursesList.TotalPages);
    }
    public async Task<PagedResponse<CourseResponse>> FilterCoursesAsync(CourseFilterRequest request)
    {
        int realCategoryId = 0;

        if(!string.IsNullOrEmpty(request.CategoryId))
        {
            realCategoryId = _protectionHandler.GetRealCategoryId(request.CategoryId);
        }
        var coursesList = await _dataStore.Courses.GetPaginatedByCriteriaAsync(request.PageSize, request.PageNumber,
            x => realCategoryId > 0 ? x.CategoryId == realCategoryId : true &&
                request.IsFree ? x.Price == 0 : true,
            x => new {x.Price, x.Id }, ["Instructor", "Category"], Sorting.Descending);

        var result = getHashedCoursesList(coursesList.ValueList);

        return PagedResponse<CourseResponse>.Create(result, coursesList.PageSize, coursesList.CurrentPage, coursesList.TotalPages);
    }
    public async Task<Result<IEnumerable<CourseChapterResponse>>> GetChaptersAsync(string courseId)
    {
        var realCourseId = _protectionHandler.GetRealCourseId(courseId);

        if (realCourseId == -1)
        {
            return Result<IEnumerable<CourseChapterResponse>>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }

        var courseChaptersList = await _dataStore.CourseChapters.GetListByCriteriaAsync(x => x.CourseId == realCourseId, ["CourseVideos"]);

        var data = convertToCourseChapterResponseList(courseChaptersList.ToList());

        return Result<IEnumerable<CourseChapterResponse>>.Success(data);
    }
    public async Task<SimpleResult<int>> CreateChapterAsync(CreateChapterRequest request)
    {
        int realCourseId = _protectionHandler.GetRealCourseId(request.CourseId);

        if (realCourseId == -1)
        {
            return SimpleResult<int>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }

        if (!await _dataStore.CourseChapters.IsExistAsync(realCourseId))
        {
            return SimpleResult<int>.Failure(ResponseMessages.COURSE_NOT_FOUND);
        }

        var chapter = new CourseChapter
        {
            CourseId = realCourseId,
            Title = request.Title,
            Description = request.Description,
        };

        await _dataStore.CourseChapters.AddAsync(chapter);

        await _dataStore.CompleteAsync();

        return SimpleResult<int>.Success(chapter.Id);
    }
    public async Task<SimpleResult<bool>> EditChapterAsync(EditChapterRequest request)
    {
        var realChapterId = _protectionHandler.GetRealChapterId(request.CourseId);

        if (realChapterId == -1)
        {
            return SimpleResult<bool>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }
        var chapter = await _dataStore.CourseChapters.GetByIdAsync(realChapterId);

        if (chapter == null)
        {
            return SimpleResult<bool>.Failure(ResponseMessages.CHAPTER_NOT_FOUND);
        }

        chapter.Title = request.Title?.Length > 0 ? request.Title : chapter.Title;

        chapter.Description = request.Description?.Length > 0 ? request.Description : chapter.Description;

        _dataStore.CourseChapters.UpdateItem(chapter);

        await _dataStore.CompleteAsync();

        return SimpleResult<bool>.Success(true);
    }
    public async Task<PagedResponse<CourseChapterResponse>> GetChaptersAsync(CourseChapterRequest request)
    {
        throw new NotImplementedException();
    }
    public async Task<SimpleResult<bool>> DeleteCourseAsync(string courseId, string webRootPath)
    {
        var realCourseId = _protectionHandler.GetRealCourseId(courseId);

        var course = await _dataStore.Courses.GetByIdAsync(realCourseId);

        if (realCourseId == -1 || !await _dataStore.Courses.IsExistAsync(realCourseId))
        {
            return SimpleResult<bool>.Failure(ResponseMessages.COURSE_NOT_FOUND);
        }

        var courseVideos = await _dataStore.CourseVideos.GetListByCriteriaAsync(x => x.CourseId == realCourseId);

        var videoPaths = courseVideos.Select(x => Path.Combine(webRootPath, x.VideoUrl)).ToList();

        _dataStore.Courses.RemoveItem(course);

        if (await _dataStore.CompleteAsync() > 0)
        {
            BackgroundJob.Enqueue(() => DeleteFile(videoPaths));
            return SimpleResult<bool>.Success(true);
        }
        return SimpleResult<bool>.Failure("Error Happened While Deleting The Course!");
    }
    public async Task<SimpleResult<int>> UploadChapterVideo(ChapterVideoRequest request, string chapterId, string courseId)
    {
        throw new NotImplementedException();
    }


    private List<CourseResponse> getHashedCoursesList(List<Course> courses)
    {
        List<CourseResponse> courseResponses = new();
        foreach (var course in courses)
        {
            courseResponses.Add(getHashedCourse(course));
        }
        return courseResponses;
    }
    private CourseResponse getHashedCourse(Course course)
    {
        return new CourseResponse(
                CourseId: _protectionHandler.GetProtectedCourseId(course.Id),
                Title: course.Title,
                Description: course.Description,
                CategoryName: course.Category.Name,
                CategoryId:  _protectionHandler.GetProtectedCategoryId(course.CategoryId),
                CourseImageUrl: course.CourseImageUrl,
                Price: course.Price,
                InstructorId: _protectionHandler.GetProtectedUserId(course.InstructorId)
            );
    }
    private List<CourseChapterResponse> convertToCourseChapterResponseList(List<CourseChapter> chapters)
    {
        List<CourseChapterResponse> result = new();
        for (int i = 0; i < chapters.Count(); i++)
        {
            List<CourseVideoResponse> courseVideoDetails = new();
            foreach (var item in chapters[i].CourseVideos)
            {
                CourseVideoResponse courseVideoResponse = new CourseVideoResponse(item.VideoUrl, item.Duration, item.Title, item.IsPreview);
                courseVideoDetails.Add(courseVideoResponse);
            }
            CourseChapterResponse chapterResponse = new CourseChapterResponse(chapters[i].Title, chapters[i].Description, chapters[i].ChapterNumber, courseVideoDetails);
            result.Add(chapterResponse);
        }
        return result;
    }
    private void DeleteFile(List<string> videoPaths)
    {
        foreach (var filePath in videoPaths)
            File.Delete(filePath);
    }
}