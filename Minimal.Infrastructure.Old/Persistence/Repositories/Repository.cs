using Microsoft.EntityFrameworkCore;
using Minimal.Domain.Entities;
using Minimal.Domain.Interfaces;

namespace Minimal.Infrastructure.Persistence.Configuration.Repositories;

public class Repository<TContext, TEntity> : IRepository<TEntity>
    where TContext : DbContext
    where TEntity : BaseEntity
{
    private readonly TContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public Repository(TContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public IQueryable<TEntity> AsQueryable() => _dbSet.AsQueryable();

    public async Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = true)
    {
        var query = _dbSet.AsQueryable();
        if (tracking)
            query = query.AsNoTracking();

        return await query.ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, bool tracking = true)
    {
        var query = _dbSet.AsQueryable();
        if (tracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
    }

    public async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);

    public async Task AddRangeAsync(IList<TEntity> entities) => await _dbSet.AddRangeAsync(entities);

    public void Update(TEntity entity) => _dbSet.Update(entity);

    public void UpdateRange(IList<TEntity> entity) => _dbSet.UpdateRange(entity);

    public void Delete(TEntity entity) => _dbSet.Remove(entity);

    public void DeleteRange(IList<TEntity> entities) => _dbSet.RemoveRange(entities);

    public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;
}
