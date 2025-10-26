using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Minimal.DAL.Entities;

namespace Minimal.Infrastructure.Persistence;

public class MinimalApiDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public MinimalApiDbContext(DbContextOptions<MinimalApiDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Add all entities configurations
        builder.ApplyConfigurationsFromAssembly(typeof(MinimalApiDbContext).Assembly);
    }
}
