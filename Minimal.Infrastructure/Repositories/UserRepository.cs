using Minimal.Domain.User;
using Minimal.Infrastructure.Persistence;

namespace Minimal.Infrastructure.Repositories;

public class UserRepository : Repository<MinimalApiDbContext, User>, IUserRepository
{
    public UserRepository(MinimalApiDbContext context) : base(context) { }
}

