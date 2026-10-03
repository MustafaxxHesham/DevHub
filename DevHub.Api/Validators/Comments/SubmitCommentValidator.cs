using DevHub.DTOS.Comments;
using FluentValidation;
namespace DevHub.Validators.Comments;
public class SubmitCommentValidator : AbstractValidator<SubmitCommentRequest>
{
    public SubmitCommentValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.PostId).NotEmpty();
        RuleFor(c => c.Content).NotEmpty()
            .MinimumLength(4);
    }
}