using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minimal.Application.Common.Interfaces;
using Minimal.DAL.Entities;
using Minimal.Domain.Interfaces;
using Minimal.Infrastructure.External.Coniugazione;
using Minimal.Infrastructure.Persistence;
using Minimal.Infrastructure.Repositories;
using Minimal.Infrastructure.Services;

namespace Minimal.Infrastructure;

public static class DependencyInjection
{
    public const string DefaultConnection = nameof(DefaultConnection);

    public static IServiceCollection AddInfrastructure<TContext>(
        this IServiceCollection services,
        IConfiguration configuration) where TContext : DbContext
    {
        // Database
        services.AddDbContext<TContext>(options =>
        {
            var connection = configuration.GetConnectionString(DefaultConnection);
            if (string.IsNullOrWhiteSpace(connection))
                throw new ConnectionStringException();

            options.UseNpgsql(connection, options =>
            {
                options.MigrationsAssembly("Minimal.DAL");
                options.EnableRetryOnFailure();
            });
        });

        // Identity
        services.AddIdentity<User, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;

            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
        }).AddEntityFrameworkStores<MinimalApiDbContext>()
        .AddDefaultTokenProviders();

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(UserRepository<>));

        // Services
        services.AddHttpClient<ConiugazioneApiClient>();
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }

    private class ConnectionStringException : Exception { }
}
