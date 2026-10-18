using DevHub.Domain.LogicContract;
using DevHub.Domain.Models;

namespace DevHub.Domain.RepositoryContract;

public interface ICourseChapterRepository : IBaseRepository<CourseChapter, int>
{
    void DeleteChapter(CourseChapter chapter);
}
