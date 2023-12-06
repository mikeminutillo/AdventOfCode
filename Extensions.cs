using NUnit.Framework;
using System.Collections;
using System.Text.RegularExpressions;

namespace AdventOfCode;

static class Extensions
{
    public static T Dump<T>(this T obj)
    {
        var output = obj?.ToString();

        if(obj is string)
        {
            // Do nothing
        }
        else if(obj is IEnumerable enumerable)
        {
            output = "";
            foreach(var item in enumerable)
            {
                output += $"{item}, ";
            }
        }

        TestContext.Out.WriteLine(output);
        return obj;
    }

    public static void EnsureFolder(string path)
    {
        if(!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public static string GetTestFolder<T>(string? root = null)
        => GetTestFolder(
                typeof(T).Namespace!.Split('.').Skip(1).Select(x => x.TrimStart('_')).First(),
                typeof(T).Namespace!.Split('.').Skip(1).Select(x => x.TrimStart('_')).Last(),
                root
            );

    public static string GetTestFolder(int year, int day, string? root = null)
        => GetTestFolder(
            $"{year}",
            $"{day:00}",
            root
        );

    public static string GetTestFolder(string year, string day, string? root = null)
        => Path.Combine(
            root ?? TestContext.CurrentContext.GetSolutionFolder(),
            year,
            day
        );

    public static string GetSolutionFolder(this TestContext testContext)
    {
        var currentDir = testContext.TestDirectory;
        while (true)
        {
            currentDir = Path.GetDirectoryName(currentDir)
                    ?? throw new Exception("No solution file found");
            if (Directory.EnumerateFiles(currentDir, "*.sln").Any())
            {
                return currentDir;
            }
        }
    }

    public static string[] AsLines(this string input)
        => input.Trim().Split("\n");

    public static int Product<T>(this IEnumerable<T> source, Func<T, int> selector)
        => source.Aggregate(1, (x, y) =>  x * selector(y));

    public static int Product(this IEnumerable<int> source)
        => source.Aggregate(1, (x, y) => x * y);

    public static IEnumerable<int> ExtractNumbers(this string source)
        => from match in Regex.Matches(source, @"\d+") select int.Parse(match.Value);

    public static IEnumerable<long> ExtractLongNumbers(this string source)
        => from match in Regex.Matches(source, @"\d+") select long.Parse(match.Value);
}
