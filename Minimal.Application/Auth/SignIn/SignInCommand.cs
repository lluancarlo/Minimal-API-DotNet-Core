using MediatR;
using Minimal.Application.Shared.Results;

namespace Minimal.Application.Auth.SignIn;

public record SignInCommand : 
    IRequest<HandlerResult<string>>, IBaseRequest, IEquatable<SignInCommand>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string Name { get; init; }
}