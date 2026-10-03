using Microsoft.AspNetCore.Mvc;
namespace DevHub.DTOS.CourseChapters;
public record class CreateChapterRequest(string Title, string Description,[FromRoute(Name = "courseId")] string CourseId);