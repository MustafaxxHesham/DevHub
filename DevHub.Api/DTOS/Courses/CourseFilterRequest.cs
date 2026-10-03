using DevHub.Domain.Enums;

namespace DevHub.DTOS.Courses;

public record class CourseFilterRequest(string? CategoryId,
    bool IsFree,
    int PageSize = 6,
    int PageNumber = 1,
    Sorting SortDirection = Sorting.Descending);