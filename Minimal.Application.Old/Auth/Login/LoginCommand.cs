using MediatR;
using Minimal.Application.Common.Results;

namespace Minimal.Application.Auth.Login;

public record LoginCommand : 
    IRequest<HandlerResult<string>>, IBaseRequest, IEquatable<LoginCommand>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}