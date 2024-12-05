namespace AdventOfCode.Infrastructure;

static class Utility
{
    public static IEnumerable<TestCaseData> AllInputs<T>(string part) =>
        LocalInputs<T>(part).Concat(PrivateInputs<T>(part));

    static IEnumerable<TestCaseData> LocalInputs<T>(string part) =>
        Path.Combine(GetTestFolder<T>(), "Input") switch
        {
            var localInputFolder => GetInputs(localInputFolder, part)
        };

    static IEnumerable<TestCaseData> PrivateInputs<T>(string part)
        => Environment.GetEnvironmentVariable("ADVENT_OF_CODE_INPUT_PATH") switch
        {
            null => [],
            var rootPath => GetTestFolder<T>(rootPath) switch
            {
                var inputPath => GetInputs(inputPath, part)
            }
        };

    static IEnumerable<TestCaseData> GetInputs(string folder, string part)
        => Directory.Exists(folder)
        ? [
            .. from path in Directory.EnumerateFiles(folder, "*.txt")
               select new TestCaseData(path).SetName($"{part}.{Path.GetFileNameWithoutExtension(path)}")
          ]
        : [];

    public static void EnsureFolder(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public static string GetTestFolder<T>(string? root = null)
        => typeof(T).Namespace!.Split('.').Skip(1).Select(x => x.TrimStart('_')).ToArray() switch
        {
        [var year, var day] => GetTestFolder(year, day, root),
            _ => throw new Exception($"Unknown namespace: {typeof(T).FullName}")
        };

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
}
