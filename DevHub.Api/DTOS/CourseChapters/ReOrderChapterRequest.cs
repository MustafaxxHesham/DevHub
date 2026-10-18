namespace DevHub.DTOS.CourseChapters;

public record class ReOrderChapterRequest(string CourseId,
string ChapterId,
int ChapterOrder);