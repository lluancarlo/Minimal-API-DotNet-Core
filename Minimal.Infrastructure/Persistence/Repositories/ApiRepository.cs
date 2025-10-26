using Minimal.Domain.Entities;
using Minimal.Domain.Interfaces;

namespace Minimal.Infrastructure.Persistence.Configuration.Repositories;

public class ApiRepository<T> : Repository<MinimalApiDbContext, T>, IRepository<T> where T : BaseEntity
{
    public ApiRepository(MinimalApiDbContext context) : base(context) { }
}

