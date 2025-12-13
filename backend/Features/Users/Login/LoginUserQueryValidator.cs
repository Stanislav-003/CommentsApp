using FluentValidation;

namespace backend.Features.Users.Login;

public class LoginUserQueryValidator : AbstractValidator<LoginUserQuery>
{
    public LoginUserQueryValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("EmailIsRequired")
            .EmailAddress().WithMessage("InvalidEmail");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("PasswordIsRequired");
    }
}