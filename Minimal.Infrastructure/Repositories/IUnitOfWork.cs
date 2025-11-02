using System.Data;

namespace Minimal.Infrastructure.Repositories;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct);
    Task<IDbTransaction> BeginTransactionAsync(CancellationToken ct);
}
