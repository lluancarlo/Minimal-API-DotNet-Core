using FluentValidation;

namespace Minimal.Application.Auth.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<UpdatePlayerCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(p => p.Email)
            .NotEmpty()
            .MaximumLength(254)
            .EmailAddress()
            .WithMessage("Email is invalid");

        RuleFor(p => p.CurrentPassword)
            .NotEmpty()
            .MaximumLength(64)
            .WithMessage("CurrentPassword is invalid");

        RuleFor(p => p.NewPassword)
            .NotEmpty()
            .MaximumLength(64)
            .WithMessage("NewPassword is invalid");
    }
}