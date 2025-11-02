using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minimal.Application.Abstraction;
using Minimal.Domain.User;
using Minimal.Infrastructure.Persistence;
using Minimal.Infrastructure.Services;

namespace Minimal.Infrastructure;

public static class DependencyInjection
{
    public const string DefaultConnection = nameof(DefaultConnection);

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDatabase<MinimalApiDbContext>(configuration);
        services.AddIdentity();

        services.AddScoped<IJwtService, JwtService>();

        return services;
    }

    private class ConnectionStringException : Exception { }


    private static void AddDatabase<TContext>(this IServiceCollection services, IConfiguration configuration)
        where TContext : DbContext
    {
        services.AddDbContext<TContext>(options =>
        {
            var connection = configuration.GetConnectionString(DefaultConnection);
            if (string.IsNullOrWhiteSpace(connection))
                throw new ConnectionStringException();

            options.UseNpgsql(connection, options =>
            {
                options.MigrationsAssembly("Minimal.Infrastructure");
                options.EnableRetryOnFailure();
            });
        }, ServiceLifetime.Scoped);

        // Register repositories here
        //services.AddScoped<IMyRepository, MyRepository>();
    }

    private static void AddIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 5;

            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
        }).AddEntityFrameworkStores<MinimalApiDbContext>()
        .AddDefaultTokenProviders();
    }
}