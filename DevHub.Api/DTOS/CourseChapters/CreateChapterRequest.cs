using DevHub.DTOS.Common;
namespace DevHub.DTOS.CourseChapters;
public record class CreateChapterRequest(string Title, 
    int ChapterNumber,
    string Description,
    string CourseId) : CourseOwner(CourseId);