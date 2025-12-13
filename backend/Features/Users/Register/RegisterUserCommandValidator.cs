using FluentValidation;

namespace backend.Features.Users.Register;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("EmailIsRequired")
            .EmailAddress().WithMessage("InvalidEmail");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("PasswordIsRequired");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("ConfirmPasswordIsRequired")
            .Equal(x => x.Password)
            .WithMessage("PasswordsDoNotMatch");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("FirstNameIsRequired");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("FirstNameIsRequired");
    }
}
