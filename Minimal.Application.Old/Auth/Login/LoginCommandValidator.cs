using FluentValidation;

namespace Minimal.Application.Auth.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(p => p.Email)
            .NotEmpty()
            .MaximumLength(254)
            .EmailAddress()
            .WithMessage("Email is invalid");

        RuleFor(p => p.Password)
            .NotEmpty()
            .MaximumLength(64)
            .WithMessage("Password is invalid");
    }
}
