using Microsoft.AspNetCore.Identity;

namespace Minimal.Domain.User;

public class User : IdentityUser<Guid>
{
    public required string Name { get; set; }
}