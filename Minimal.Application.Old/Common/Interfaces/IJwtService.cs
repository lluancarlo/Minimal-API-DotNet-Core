using Minimal.Application.Common.Results;

namespace Minimal.Application.Common.Interfaces;

public interface IJwtService
{
    ServiceResult<string> GenerateToken(Guid userId, string email, ICollection<string> roles);
}