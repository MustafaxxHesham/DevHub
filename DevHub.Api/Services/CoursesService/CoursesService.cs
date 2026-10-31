using DevHub.AuthorizationRequirements;
using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Enums;
using DevHub.Domain.Models;
using DevHub.Domain.Result;
using DevHub.DTOS.Commons;
using DevHub.DTOS.CourseChapters;
using DevHub.DTOS.Courses;
using DevHub.DTOS.CourseVideos;
using DevHub.Responses;
using DevHub.Utilities;
using Hangfire;
using System.Security.Claims;

namespace DevHub.Services.CoursesService;

public class CoursesService(IDataStore _dataStore, ProtectionHandler _protectionHandler) : ICoursesService
{
    // Courses Handling
    public async Task<Result<CourseResponse>> CreateCourseAsync(CreateCourseRequest request, string webRootPath)
    {
        var categoryId = _protectionHandler.GetRealCategoryId(request.CategoryId);

        var userId = _protectionHandler.GetRealUserId(request.UserId);

        bool[] isAllExist = await Task.WhenAll(_dataStore.Categories.IsExistAsync(categoryId),
                                               _dataStore.Users.IsExistAsync(userId));
        if (!isAllExist[0])
        {
            var errorMessage = nameof(Category) + " " + (isAllExist[1] ? string.Empty : nameof(User));
            return Result<CourseResponse>.Failure(ResponseMessages.EntityNotFound(errorMessage));
        }

        var course = new Course
        {
            CategoryId = categoryId,
            InstructorId = userId,
            Description = request.Description,
            Price = request.Price,
            CourseImageUrl = await UploadImageFileToServerAsync(request.CourseImageFile, webRootPath),
            IsPublished = request.IsPublished,
        };

        await _dataStore.Courses.AddAsync(course);

        await _dataStore.CompleteAsync();

        return Result<CourseResponse>.Success(GetHashedCourse(course));
    }
    public async Task<Result<CourseResponse>> GetCourseByIdAsync(string courseId)
    {
        var realCourseId = _protectionHandler.GetRealCourseId(courseId);
        
        if (realCourseId == -1)
        {
            return Result<CourseResponse>.Failure(ResponseMessages.COURSE_NOT_FOUND);
        }

        var course = await _dataStore.Courses.GetCourseById(realCourseId);

        if (course == null)
        {
            return Result<CourseResponse>.Failure(ResponseMessages.COURSE_NOT_FOUND);
        }

        return Result<CourseResponse>.Success(GetHashedCourse(course));
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

        var result = GetHashedCoursesList(res.ValueList);

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

        var courseResponses = GetHashedCoursesList(coursesList.ValueList);

        var data = PagedResponse<CourseResponse>.Create(courseResponses, coursesList.PageSize, coursesList.CurrentPage, coursesList.TotalPages);

        return Result<PagedResponse<CourseResponse>>.Success(data);
    }
    public async Task<PagedResponse<CourseResponse>> GetCoursesAsync(PagedRequest request)
    {
        var pagedList = await _dataStore.Courses.GetBestsellersAsync(request.PageSize, request.PageNumber, request.LastId);

        var coursesResponse = GetHashedCoursesList(pagedList.ValueList);

        var response = PagedResponse<CourseResponse>.Create(coursesResponse, pagedList.PageSize, pagedList.CurrentPage, pagedList.TotalPages);

        return response;
    }
    public async Task<PagedResponse<CourseResponse>> SearchCoursesAsync(KeyPagedRequest<string> request)
    {
        var coursesList = await _dataStore.Courses.GetPaginatedByCriteriaAsync(request.PageSize,
            request.PageNumber,
            x => x.Title.Contains(request.Key) || x.Description.Contains(request.Key),
            x => new {x.Price, x.Id }, ["Instructor", "Category"], Sorting.Descending);

        var result = GetHashedCoursesList(coursesList.ValueList);

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
            x => realCategoryId > 0 ? x.CategoryId == realCategoryId : true && request.IsFree ? x.Price == 0 : true,
            x => new {x.Price, x.Id }, ["Instructor", "Category"], Sorting.Descending);

        var result = GetHashedCoursesList(coursesList.ValueList);

        return PagedResponse<CourseResponse>.Create(result, coursesList.PageSize, coursesList.CurrentPage, coursesList.TotalPages);
    }
    public async Task<Result<PagedResponse<CourseResponse>>> GetWishListCoursesForUser(KeyPagedRequest<string> request)
    {
        var realUserId = _protectionHandler.GetRealUserId(request.Key);

        if (realUserId == -1 || await _dataStore.Users.IsExistAsync(realUserId))
        {
            return Result<PagedResponse<CourseResponse>>.Failure(ResponseMessages.EntityNotFound(nameof(User)));
        }

        var pagedResult = await _dataStore.WishLists.GetPaginatedByCriteriaAsync(
            request.PageSize,
            request.PageNumber,
            w => w.UserId == realUserId,
            w => w.UserId,
            [nameof(Course)]);

        var courses = pagedResult.ValueList.Select(w => w.Course);

        var courseResponseList = GetHashedCoursesList(courses.ToList());

        var result = PagedResponse<CourseResponse>.Create(courseResponseList, pagedResult.PageSize, pagedResult.CurrentPage, pagedResult.TotalPages);

        return Result<PagedResponse<CourseResponse>>.Success(result);
    }
    public async Task<Result<PagedResponse<EnrolledCourseResponse>>> GetEnrolledCoursesAsync(KeyPagedRequest<string> request)
    {
        var realUserId = _protectionHandler.GetRealUserId(request.Key);

        if (realUserId == -1)
        {
            return Result<PagedResponse<EnrolledCourseResponse>>.Failure(ResponseMessages.USER_NOT_FOUND);
        }

        var pagedData = await _dataStore.Courses.GetEnrolledCoursesAsync(request.PageSize, request.PageNumber, realUserId,
            request.LastKeyId);

        List<EnrolledCourseResponse> response = new List<EnrolledCourseResponse>();

        foreach (var item in pagedData.ValueList)
        {
            response.Add(GetEnrolledCourseResponse(item.Item1, item.Item2));
        }

        var result = PagedResponse<EnrolledCourseResponse>.Create(response, pagedData.PageSize, pagedData.CurrentPage, pagedData.TotalPages);

        return Result<PagedResponse<EnrolledCourseResponse>>.Success(result);
    }
    public async Task<SimpleResult<int>> EnrollCourseAsync(EnrollCourseRequest request)
    {
        var userId = _protectionHandler.GetRealUserId(request.UserId);

        if (userId == -1 || !await _dataStore.Users.IsExistAsync(userId))
        {
            return SimpleResult<int>.Failure(ResponseMessages.EntityNotFound(nameof(User)));
        }
        var courseId = _protectionHandler.GetRealCourseId(request.CourseId);

        if (courseId == -1 || !await _dataStore.Courses.IsExistAsync(courseId))
        {
            return SimpleResult<int>.Failure(ResponseMessages.EntityNotFound(nameof(Course)));
        }

        if (await _dataStore.UserCourses.IsExistWithCriteriaAsync(x => x.CourseId == courseId && x.UserId == userId))
        {
            return SimpleResult<int>.Failure(ResponseMessages.ACTION_ALREADY_DONE);
        }

        //if (await _dataStore.Transactions.IsExistWithCriteriaAsync(true)) Validate It.

        var userCourse = new UserCourse
        {
            CourseId = courseId,
            UserId = userId,
            EnrolledAt = DateTime.UtcNow
        };


        await _dataStore.UserCourses.AddAsync(userCourse);
        await _dataStore.CompleteAsync();

        return SimpleResult<int>.Success(userCourse.Id);
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
    public async Task<SimpleResult<bool>> EditCourseAsync(EditCourseRequest request, string webRootPath)
    {
        var courseId = _protectionHandler.GetRealCourseId(request.CourseId);
        
        var userId = _protectionHandler.GetRealUserId(request.UserId);

        var course = await _dataStore.Courses.GetByIdAsync(courseId);

        course.Title = string.IsNullOrEmpty(request.Title) ? course.Title: request.Title.Trim();

        course.Description = string.IsNullOrEmpty(request.Description) ? course.Description: request.Description.Trim();
        
        course.Price = request.Price.HasValue ? request.Price.Value: course.Price;

        course.IsPublished = request.IsPublished.HasValue ? request.IsPublished.Value: course.IsPublished;

        if (request.CourseImageFile is not null)
        {
            course.CourseImageUrl = await UploadImageFileToServerAsync(request.CourseImageFile, webRootPath);
        }

        if (!string.IsNullOrEmpty(request.CategoryId))
        {
            int categoryId = _protectionHandler.GetRealCategoryId(request.CategoryId);

            if (categoryId == -1 || !await _dataStore.Categories.IsExistAsync(categoryId))
            {
                return SimpleResult<bool>.Failure(ResponseMessages.EntityNotFound(nameof(Category)));
            }

            course.CategoryId = categoryId;
        }

        return SimpleResult<bool>.Success(true);
    }
    public async Task<bool> IsUserCourseInstructorAsync(ClaimsPrincipal user, string courseId)
    {
        var userId = Common.RetrieveUserId(user, _protectionHandler);

        if (userId == null || userId < 0)
        {
            return false;
        }

        return await _dataStore.Courses.IsExistWithCriteriaAsync(c => c.InstructorId == userId);
    }

    // Chapters Handling
    public async Task<Result<IEnumerable<CourseChapterResponse>>> GetChaptersAsync(string courseId)
    {
        var realCourseId = _protectionHandler.GetRealCourseId(courseId);

        if (realCourseId == -1)
        {
            return Result<IEnumerable<CourseChapterResponse>>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }

        var courseChaptersList = await _dataStore.CourseChapters.GetListByCriteriaAsync(x => x.CourseId == realCourseId, ["CourseVideos"]);

        var data = ConvertToCourseChapterResponseList(courseChaptersList.ToList());

        return Result<IEnumerable<CourseChapterResponse>>.Success(data);
    }
    public async Task<Result<CourseChapterResponse>> CreateChapterAsync(CreateChapterRequest request)
    {
        int realCourseId = _protectionHandler.GetRealCourseId(request.CourseId);

        if (realCourseId == -1)
        {
            return Result<CourseChapterResponse>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }

        if (!await _dataStore.CourseChapters.IsExistAsync(realCourseId))
        {
            return Result<CourseChapterResponse>.Failure(ResponseMessages.COURSE_NOT_FOUND);
        }

        var chapter = new CourseChapter
        {
            CourseId = realCourseId,
            Title = request.Title,
            Description = request.Description,
            ChapterNumber = request.ChapterNumber
        };

        await _dataStore.CourseChapters.AddAsync(chapter);

        await _dataStore.CompleteAsync();

        return Result<CourseChapterResponse>.Success(ConvertToCourseChapterResponse(chapter));
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
    public async Task<SimpleResult<int>> UploadChapterVideo(ChapterVideoRequest request, string chapterId, string courseId)
    {
        throw new NotImplementedException();
    }
    public async Task<SimpleResult<bool>> DeleteChapterAsync(string courseId, string chapterId, string webRootPath)
    {
        var realCourseId = _protectionHandler.GetRealCourseId(courseId);

        if (realCourseId == -1)
        {
            return SimpleResult<bool>.Failure(""); // error
        }

        var realChapterId = _protectionHandler.GetRealChapterId(chapterId);

        if (realChapterId == -1)
        {
            return SimpleResult<bool>.Failure(""); // error
        }

        var chapter = await _dataStore.CourseChapters.GetByCriteriaFirstAsync(c => c.CourseId == realCourseId
        && c.Id == realChapterId, ["CourseVideos"]);

        if (chapter is null)
        {
            return SimpleResult<bool>.Failure("Item Not Found");
        }

        _dataStore.CourseChapters.DeleteChapter(chapter);

        await _dataStore.CompleteAsync();

        return SimpleResult<bool>.Success(true);
    }

    // to be tested
    public async Task<SimpleResult<bool>> ReOrderChapterAsync(ReOrderChapterRequest request)
    {
        var chapterId = _protectionHandler.GetRealChapterId(request.ChapterId);

        if (chapterId == -1)
        {
            return SimpleResult<bool>.Failure(ResponseMessages.CHAPTER_NOT_FOUND);
        }

        var courseId = _protectionHandler.GetRealCourseId(request.CourseId);

        if (courseId == -1)
        {
            return SimpleResult<bool>.Failure(ResponseMessages.COURSE_NOT_FOUND);
        }

        List<CourseChapter> chaptersList = (await _dataStore.CourseChapters.GetListByCriteriaAsync(c => c.CourseId == courseId)).ToList();

        if (chaptersList is null)
        {
            return SimpleResult<bool>.Failure(ResponseMessages.CHAPTER_NOT_FOUND);
        }

        var chapterToUpdate = chaptersList.Where(c => c.Id == chapterId);

        if (chaptersList is null)
        {
            return SimpleResult<bool>.Failure(ResponseMessages.CHAPTER_NOT_FOUND);
        }

        var orderedList = ReOrderChapter(chaptersList, chapterId, request.ChapterOrder);

        foreach (var item in orderedList)
        {
            _dataStore.CourseChapters.UpdateItem(item);
        }

        await _dataStore.CompleteAsync();

        return SimpleResult<bool>.Success(true);
    }
    
    private async Task<string> UploadImageFileToServerAsync(IFormFile image, string webRootPath)
    {
        string imageName = Guid.NewGuid().ToString() + Path.GetFileName(image.FileName);

        string imagePath = string.Empty;

        imagePath = Path.Combine(webRootPath, "Courses Thumbnails", imageName);

        using (var fs = new FileStream(imagePath, FileMode.Create))
            await image.CopyToAsync(fs);

        return imagePath;
    }
    private void DeleteFile(List<string> videoPaths)
    {
        foreach (var filePath in videoPaths)
            File.Delete(filePath);
    }
    private CourseResponse GetHashedCourse(Course course)
    {
        return new CourseResponse(
                CourseId: _protectionHandler.GetProtectedCourseId(course.Id),
                Title: course.Title,
                Description: course.Description,
                CategoryName: course.Category.Name,
                CategoryId: _protectionHandler.GetProtectedCategoryId(course.CategoryId),
                CourseImageUrl: course.CourseImageUrl,
                Price: course.Price,
                InstructorId: _protectionHandler.GetProtectedUserId(course.InstructorId)
            );
    }
    private EnrolledCourseResponse GetEnrolledCourseResponse(Course course, double completionProgress)
    {
        return new EnrolledCourseResponse(
                CourseId: _protectionHandler.GetProtectedCourseId(course.Id),
                Title: course.Title,
                Description: course.Description,
                CategoryName: course.Category.Name,
                CategoryId: _protectionHandler.GetProtectedCategoryId(course.CategoryId),
                CourseImageUrl: course.CourseImageUrl,
                ProgressPercentage: (int)completionProgress
            );
    }
    private List<CourseResponse> GetHashedCoursesList(List<Course> courses)
    {
        List<CourseResponse> courseResponses = new();
        foreach (var course in courses)
        {
            courseResponses.Add(GetHashedCourse(course));
        }
        return courseResponses;
    }
    private CourseChapterResponse ConvertToCourseChapterResponse(CourseChapter chapter)
    {
        var courseVideoList = new List<CourseVideoResponse>();
        foreach (var item in chapter.CourseVideos)
        {
            CourseVideoResponse courseVideoResponse = new CourseVideoResponse(item.VideoUrl, item.Duration, item.Title, item.IsPreview);
            courseVideoList.Add(courseVideoResponse);
        }

        return new CourseChapterResponse(_protectionHandler.GetProtectedChapterId(chapter.Id), 
            chapter.Title, 
            chapter.Description, 
            chapter.ChapterNumber,
            courseVideoList);
    }
    private List<CourseChapterResponse> ConvertToCourseChapterResponseList(List<CourseChapter> chapters)
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
            CourseChapterResponse chapterResponse = new CourseChapterResponse(
                _protectionHandler.GetProtectedChapterId(chapters[i].Id),
                chapters[i].Title, chapters[i].Description, chapters[i].ChapterNumber, courseVideoDetails);
            result.Add(chapterResponse);
        }
        return result;
    }
    private List<CourseChapter> ReOrderChapter(List<CourseChapter> list, int chapterId, int order)
    {
        var chapterToReOrder = list.Where(c => c.Id == chapterId).SingleOrDefault()!;
        var chapterWithRequiredPosition = list.Where(c => c.ChapterNumber == order).SingleOrDefault()!;

        int requiredPosition = list.IndexOf(chapterWithRequiredPosition);
        int chapterIndex = list.IndexOf(chapterToReOrder);

        chapterToReOrder.ChapterNumber = chapterWithRequiredPosition.ChapterNumber;

        if (chapterIndex > requiredPosition)
        {
            for (int i = requiredPosition; i < chapterIndex && i < list.Count; i++)
            {
                list[i].ChapterNumber += 1;
            }
            list.RemoveAll(x => x.Id == chapterToReOrder.Id);
            list.Insert(requiredPosition, chapterToReOrder);
        }

        else if (chapterIndex < requiredPosition)
        {
            for (int i = chapterIndex + 1; i < requiredPosition; i++)
            {
                list[i].ChapterNumber -= 1;
            }
            list.RemoveAll(x => x.Id == chapterToReOrder.Id);
            list.Insert(requiredPosition, chapterToReOrder);
        }
        return list;
    }

    public async Task<Result<CourseChapterResponse>> GetChapterByIdAsync(string chapterId)
    {
        var realChapterId = _protectionHandler.GetRealChapterId(chapterId);

        if (realChapterId == -1)
        {
            Result<CourseChapterResponse>.Failure(ResponseMessages.CHAPTER_NOT_FOUND);
        }

        var chapter = await _dataStore.CourseChapters.GetByIdAsync(realChapterId);

        if (chapter is null)
        {
            Result<CourseChapterResponse>.Failure(ResponseMessages.CHAPTER_NOT_FOUND);
        }

        var chapterResponse = ConvertToCourseChapterResponse(chapter);

        return Result<CourseChapterResponse>.Success(chapterResponse);
    }
}