using Minimal.Application.Shared.Results;

namespace Minimal.Application.Abstraction;

public interface IJwtService
{
    ServiceResult<string> GenerateToken(Guid userId, string email, ICollection<string> roles);
}