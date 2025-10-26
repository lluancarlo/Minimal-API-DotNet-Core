using Microsoft.AspNetCore.Identity;

namespace Minimal.DAL.Entities;

public class User : IdentityUser<Guid>
{
    public required string Name { get; set; }
}