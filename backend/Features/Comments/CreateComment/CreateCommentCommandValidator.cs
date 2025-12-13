using FluentValidation;

namespace backend.Features.Comments.CreateComment;

public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.ParentId)
            .Must(id => id == null || id != Guid.Empty)
            .WithMessage("ParentIdWhenProvidedMustBeAValidGUID");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserIdMustBeProvided");

        RuleFor(x => x.CaptchaCode)
            .NotEmpty()
            .WithMessage("CaptchaCodeMustBeProvided");

        RuleFor(x => x.CaptchaId)
            .NotEmpty()
            .WithMessage("CaptchaIdMustBeProvided");

        RuleFor(x => x.CaptchaId)
            .NotEmpty()
            .WithMessage("TextIsRequired");
    }
}