using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Minimal.Infrastructure.Persistence
{
    public class MinimalApiDbContextFactory : IDesignTimeDbContextFactory<MinimalApiDbContext>
    {
        public MinimalApiDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Minimal.Api");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString(DependencyInjection.DefaultConnection);

            var optionsBuilder = new DbContextOptionsBuilder<MinimalApiDbContext>();
            optionsBuilder.UseNpgsql(connectionString, o =>
            {
                o.MigrationsAssembly("Minimal.Infrastructure");
                o.EnableRetryOnFailure();
            });

            return new MinimalApiDbContext(optionsBuilder.Options);
        }
    }
}
