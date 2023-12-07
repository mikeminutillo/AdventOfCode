using System.Net;

namespace AdventOfCode;

[TestFixture]
public class Helpers
{
    static int Year = DateTime.Now.Year;
    static int Day = DateTime.Now.Day;

    //static int Year = 2015;
    //static int Day = 1;

    [Test, Explicit]
    public Task Setup()
        => Task.WhenAll(
            GrabInput(Year, Day),
            SetupDay(Year, Day));

    private async Task GrabInput(int year, int day)
    {
        var baseAddress = new Uri("https://adventofcode.com/");
        var cookieContainer = new CookieContainer();
        var session = Environment.GetEnvironmentVariable("ADVENT_OF_CODE_SESSION_KEY");

        if(session is null)
        {
            "Add an environment variable with key ADVENT_OF_CODE_SESSION_KEY containing the contents of your session cookie".Dump();
            return;
        }

        var inputDir = Environment.GetEnvironmentVariable("ADVENT_OF_CODE_INPUT_PATH");
        if(inputDir is null)
        {
            "Add an environment variable with key ADVENT_OF_CODE_INPUT_PATH with the path to store your input files. THESE SHOULD NOT BE MADE PUBLIC!".Dump();
            return;
        }

        var outputDir = Extensions.GetTestFolder(year, day, inputDir);;
        Extensions.EnsureFolder(outputDir);

        using var handler = new HttpClientHandler { CookieContainer = cookieContainer };
        using var client = new HttpClient(handler) { BaseAddress = baseAddress };

        cookieContainer.Add(baseAddress, new Cookie("session", session));
        var result = await client.GetStringAsync($"/{year}/day/{day}/input");

        await File.WriteAllTextAsync(Path.Combine(outputDir, "input.txt"), result);
    }

    private async Task SetupDay(int year, int day)
    {
        var testFolder = Extensions.GetTestFolder(year, day);
        Extensions.EnsureFolder(testFolder);

        var solverClassName = $"Day{day:00}";
        var solverFilePath = Path.Combine(testFolder, $"{solverClassName}.cs");

        if(!File.Exists(solverFilePath))
        {
            await File.WriteAllTextAsync(solverFilePath, $@"namespace AdventOfCode._{year}._{day:00};

public class {solverClassName} : AdventOfCodeBase<{solverClassName}>
{{
}}");
        }

        var inputFolder = Path.Combine(testFolder, "Input");
        Extensions.EnsureFolder(inputFolder);

        var inputFilePath = Path.Combine(inputFolder, "sample.txt");

        if(!File.Exists(inputFilePath))
        {
            await File.WriteAllTextAsync(inputFilePath, string.Empty);
        }
    }
}
