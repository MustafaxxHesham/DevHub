using Microsoft.AspNetCore.Mvc;

namespace DevHub.DTOS.CourseChapters;

public record class EditChapterRequest([FromRoute(Name = "courseId")]string CourseId,
    string? Title, 
    string? Description,
    int? ChapterOrder);