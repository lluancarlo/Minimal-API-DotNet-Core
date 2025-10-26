using Impoter.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Impoter;

public static class DependencyInjection
{
    public static IServiceCollection AddImporterServices(this IServiceCollection services)
    {
        //services.AddHttpClient("ConjugationClient", client =>
        //{
        //    client.BaseAddress = new Uri("https://www.coniugazione.it/");
        //    client.DefaultRequestHeaders.UserAgent.ParseAdd("ImporterApp/1.0");
        //});

        services.AddScoped<IImporterService, ImporterService>();

        return services;
    }
}
