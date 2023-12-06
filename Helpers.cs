using NUnit.Framework;
using System.Net;

namespace AdventOfCode;

[TestFixture]
public class Helpers
{
    [Test, Explicit]
    public Task GrabOtherInput()
        => GrabInput(2015, 1);

    [Test, Explicit]
    public Task GrabTodaysInput()
        => GrabInput(DateTimeOffset.Now.Year, DateTimeOffset.Now.Day);

    private async Task GrabInput(int year, int day)
    {
        var baseAddress = new Uri("https://adventofcode.com/");
        var cookieContainer = new CookieContainer();
        var session = Environment.GetEnvironmentVariable("ADVENT_OF_CODE_SESSION_KEY")!;
        var inputDir = Environment.GetEnvironmentVariable("ADVENT_OF_CODE_INPUT_PATH")!;

        using var handler = new HttpClientHandler { CookieContainer = cookieContainer };
        using var client = new HttpClient(handler) { BaseAddress = baseAddress };

        cookieContainer.Add(baseAddress, new Cookie("session", session));
        var result = await client.GetStringAsync($"/{year}/day/{day}/input");

        var outputDir = Path.Combine(inputDir, $"{year}", $"{day:00}");

        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        await File.WriteAllTextAsync(Path.Combine(outputDir, "input.txt"), result);
    }
}
