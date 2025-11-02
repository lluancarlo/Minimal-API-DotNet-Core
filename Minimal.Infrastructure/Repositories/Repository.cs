using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Minimal.Infrastructure.Repositories;

public abstract class Repository<TContext, TEntity>
    where TContext : DbContext
    where TEntity : class
{
    protected readonly TContext DbContext;
    private readonly DbSet<TEntity> _dbSet;

    public Repository(TContext context)
    {
        DbContext = context;
        _dbSet = context.Set<TEntity>();
    }

    public IQueryable<TEntity> AsQueryable() => _dbSet.AsQueryable();

    public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, bool tracking = false, params Expression<Func<TEntity, object>>[] includeProperties) =>
        All(tracking, includeProperties).Where(predicate);

    public IQueryable<TEntity> All(bool tracking = false, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        var query = _dbSet.AsQueryable();

        if (includeProperties.Any())
            foreach (var includeProperty in includeProperties)
                query = query.Include(includeProperty);

        if (tracking)
            query = query.AsNoTracking();

        return query;
    }

    public async Task<TEntity?> GetByKeyAsync(Guid id, CancellationToken ct, bool tracking = false)
    {
        var query = _dbSet.AsQueryable();
        if (tracking)
            query = query.AsNoTracking();

        return await query.FirstAsync(e => EF.Property<Guid>(e, "Id") == id, ct);
    }

    public async Task Add(TEntity entity, CancellationToken ct) =>
        await _dbSet.AddAsync(entity, ct);

    public async Task AddRange(IList<TEntity> entities, CancellationToken ct) =>
        await _dbSet.AddRangeAsync(entities, ct);

    public void Update(TEntity entity) =>
        _dbSet.Update(entity);

    public void UpdateRange(IList<TEntity> entity) =>
        _dbSet.UpdateRange(entity);

    public void Delete(TEntity entity) =>
        _dbSet.Remove(entity);

    public void DeleteRange(IList<TEntity> entities) =>
        _dbSet.RemoveRange(entities);
}
