using MediatR;
using Microsoft.AspNetCore.Identity;
using Minimal.Application.Common.Interfaces;
using Minimal.Application.Common.Results;
using Minimal.DAL.Entities;

namespace Minimal.Application.Auth.SignIn;

public class SignInCommandHandler : IRequestHandler<SignInCommand, HandlerResult<string>>
{
    private readonly IJwtService jwtService;
    private readonly SignInManager<User> signInManager;
    private readonly UserManager<User> userManager;

    public SignInCommandHandler(IJwtService _jwtService,
        SignInManager<User> _signInManager,
        UserManager<User> _userManager)
    {
        jwtService = _jwtService;
        signInManager = _signInManager;
        userManager = _userManager;
    }

    public async Task<HandlerResult<string>> Handle(SignInCommand command, CancellationToken ct)
    {
        var validatorResult = new SignInCommandValidator().Validate(command);
        if (!validatorResult.IsValid)
            return HandlerResult<string>.Failure(validatorResult.Errors.Select(s => s.ErrorMessage).ToList());

        var user = await userManager.FindByEmailAsync(command.Email);
        if (user != null)
            return HandlerResult<string>.Failure("Email already in use");

        user = new User
        {
            Name = command.Name,
            UserName = command.Email,
            Email = command.Email,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
            return HandlerResult<string>.Failure("Error while creating user: " + string.Join(" | ", result.Errors.Select(s => $"{s.Code} - {s.Description}")));

        var signInResult = await signInManager.PasswordSignInAsync(user, command.Password, false, false);
        if (!signInResult.Succeeded)
        {
            if (signInResult.IsLockedOut)
                return HandlerResult<string>.Failure("User account is locked.");
            if (signInResult.IsNotAllowed)
                return HandlerResult<string>.Failure("User is not allowed to sign in.");
            return HandlerResult<string>.Failure("Invalid login attempt.");
        }

        var roles = await userManager.GetRolesAsync(user);
        var serviceReponse = jwtService.GenerateToken(user.Id, user.Email, roles);

        if (!serviceReponse.IsSuccess)
            return HandlerResult<string>.Failure("Error while login: " + result.ToString());

        return HandlerResult<string>.Success(serviceReponse.Data!);
    }
}