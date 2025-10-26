using MediatR;
using Minimal.Application.Common.Results;

namespace Minimal.Application.Auth.ChangePassword;

public record UpdatePlayerCommand :
    IRequest<HandlerResult>, IBaseRequest, IEquatable<UpdatePlayerCommand>
{
    public required string Email { get; init; }
    public required string CurrentPassword { get; init; }
    public required string NewPassword { get; init; }
}