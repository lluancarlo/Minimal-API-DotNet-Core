using Generator.Common.Records;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Minimal.DAL;
using Minimal.DAL.Contexts;

namespace BigGenerator.Generators;

public abstract class BaseGenerator
{
    public readonly GeneratorDbContext Context;

    protected BaseGenerator()
    {
        var build = Host.CreateDefaultBuilder()
            .ConfigureServices(static (context, services) =>
            {
                DataAccessLayerExtension.LoadServices<GeneratorDbContext>(services, context.Configuration);
            })
            .Build();

        Context = build.Services.CreateScope().ServiceProvider.GetRequiredService<GeneratorDbContext>();
    }

    public abstract Task<GeneratorResponse> Run();
}
