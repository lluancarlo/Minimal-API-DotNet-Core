//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Impoter.Services;
//using Impoter;

//var builder = Host.CreateDefaultBuilder(args)
//    .ConfigureServices((context, services) =>
//    {
//        // Register IHttpClientFactory support
//        services.AddHttpClient();
//        services.AddImporterServices();
//    })
//    .Build();

//Console.WriteLine("Starting the importer...");

//using var scope = builder.Services.CreateScope();
//var importer = scope.ServiceProvider.GetRequiredService<IImporterService>();

//var result = await importer.GetVerbConjugation("test123");
//Console.WriteLine("Import successful!");
