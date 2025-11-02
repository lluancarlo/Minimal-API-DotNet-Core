using MediatR;
using Microsoft.AspNetCore.Identity;
using Minimal.Application.Shared.Results;
using Minimal.Domain.User;

namespace Minimal.Application.Auth.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<UpdatePlayerCommand, HandlerResult>
{
    private readonly SignInManager<User> signInManager;
    private readonly UserManager<User> userManager;

    public ChangePasswordCommandHandler(SignInManager<User> _signInManager,
        UserManager<User> _userManager)
    {
        signInManager = _signInManager;
        userManager = _userManager;
    }
    public async Task<HandlerResult> Handle(UpdatePlayerCommand command, CancellationToken ct)
    {
        var validatorResult = new ChangePasswordCommandValidator().Validate(command);
        if (!validatorResult.IsValid)
            return HandlerResult.Failure(validatorResult.Errors.Select(s => s.ErrorMessage).ToList());

        var user = await userManager.FindByEmailAsync(command.Email);
        if (user == null)
            return HandlerResult.Failure("User not found.");

        var passwordCheck = await userManager.CheckPasswordAsync(user, command.CurrentPassword);
        if (!passwordCheck)
            return HandlerResult.Failure("Current password is incorrect.");

        var result = await userManager.ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return HandlerResult.Failure($"Failed to change password: {errors}");
        }

        // Optional: Sign the user out everywhere or force re-login
        await signInManager.RefreshSignInAsync(user);

        return HandlerResult.Success();
    }
}