namespace DevHub.DTOS.CourseChapters;

public record class EditChapterRequest(string CourseId, string? Title, string? Description);