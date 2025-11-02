using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Minimal.Domain.User;
using Minimal.Infrastructure.Repositories;
using System.Data;

namespace Minimal.Infrastructure.Persistence;

public class MinimalApiDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>, IUnitOfWork
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

    public override async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        try
        {
            return await base.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new DBConcurrencyException("A concurrency error occurred while saving changes to the database.", ex);
        }
    }

    public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken ct)
    {
        var dbTransaction = await Database.BeginTransactionAsync(ct);
        return dbTransaction.GetDbTransaction();
    }
}
