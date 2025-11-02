using MediatR;
using Microsoft.AspNetCore.Identity;
using Minimal.Application.Abstraction;
using Minimal.Application.Shared.Results;
using Minimal.Domain.User;

namespace Minimal.Application.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, HandlerResult<string>>
{
    private readonly IJwtService _jwtService;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public LoginCommandHandler(
        IJwtService jwtService,
        SignInManager<User> signInManager,
        UserManager<User> userManager)
    {
        _jwtService = jwtService;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<HandlerResult<string>> Handle(LoginCommand command, CancellationToken ct)
    {
        var validatorResult = new LoginCommandValidator().Validate(command);
        if (!validatorResult.IsValid)
            return HandlerResult<string>.Failure(validatorResult.Errors.Select(s => s.ErrorMessage).ToList());
            //return HandlerResult.ValidatorError(validatorResult.Errors.Select(s => s.ErrorMessage));

        var user = await _userManager.FindByEmailAsync(command.Email);
        if (user == null)
            return HandlerResult<string>.Failure("User does not exist");

        var result = await _signInManager.CheckPasswordSignInAsync(user, command.Password, false);

        if (!result.Succeeded)
            return HandlerResult<string>.Failure("Error while login: " + result.ToString());

        var roles = await _userManager.GetRolesAsync(user);
        var serviceReponse = _jwtService.GenerateToken(user.Id, user.Email!, roles);

        if (!serviceReponse.IsSuccess)
            return HandlerResult<string>.Failure("Error while login: " + result.ToString());

        return HandlerResult<string>.Success(serviceReponse.Data ?? string.Empty);
    }
}