using DevHub.DTOS.Courses;
using FluentValidation;

namespace DevHub.Validators.Courses;

public class CreateCourseRequestValidator : AbstractValidator<CreateCourseRequest>
{
    public CreateCourseRequestValidator()
    {
        RuleFor(c => c.Title)
            .MinimumLength(20);
    }
}
