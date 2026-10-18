using Data.Layer.EFCore;
using Data.Layer.EFCore.Repository;
using DevHub.Domain.Models;
using DevHub.Domain.RepositoryContract;

namespace DevHub.EFCore.Repository;

public class CourseChapterRepository(AppDbContext _context) : BaseRepository<CourseChapter, int>(_context), ICourseChapterRepository
{
    public void DeleteChapter(CourseChapter chapter)
    {
        var courseVideos = _context.CourseVideos.Where(c => c.CourseChapterId == chapter.Id);
        _context.CourseVideos.RemoveRange(courseVideos);
        _context.CourseChapters.Remove(chapter);
    }
}