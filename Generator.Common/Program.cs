using BigGenerator.Generators;

namespace Generator.Common;

public class Program
{
    // Add generator here to make it run
    private static readonly BaseGenerator[] GeneratorsToRun = [
        new CountriesGenerator(),
        //new EnglishWordsGenerator()
    ];

    static async Task<int> Main() => await RunAllGenerators();

    private static async Task<int> RunAllGenerators()
    {
        Console.WriteLine($" -> Running all generators project !");

        foreach(var generator in GeneratorsToRun)
            await RunGenerator(generator);

        Console.WriteLine(" -> Done all generations. Press Enter to close");
        Console.ReadKey();

        return 0;
    }

    private static async Task RunGenerator(BaseGenerator generator)
    {
        var generatorName = generator.GetType().Name;
        var response = await generator.Run();

        if (response.Success)
            Console.WriteLine($"   [ OKAY  ]   Generator {generatorName} done. {response.Message}   ");
        else
            Console.WriteLine($"   [ ERROR ]   Generator {generatorName} error message: {response.Message}   ");
    }
}