using FluentValidation;

namespace Minimal.Application.Auth.SignIn;

public class SignInCommandValidator : AbstractValidator<SignInCommand>
{
    public SignInCommandValidator()
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

        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Name is invalid");
    }
}