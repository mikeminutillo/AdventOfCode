namespace AdventOfCode.Infrastructure;

static class Utility
{
    public static IEnumerable<TestCaseData> AllInputs<T>(string part) =>
        LocalInputs<T>(part).Concat(PrivateInputs<T>(part));

    static IEnumerable<TestCaseData> LocalInputs<T>(string part) =>
        from path in Directory.EnumerateFiles(
            Path.Combine(GetTestFolder<T>(), "Input"),
            "*.txt")
        select new TestCaseData(path).SetName($"{part}.{Path.GetFileNameWithoutExtension(path)}");

    static IEnumerable<TestCaseData> PrivateInputs<T>(string part)
    {
        var rootPath = Environment.GetEnvironmentVariable("ADVENT_OF_CODE_INPUT_PATH");
        if (rootPath is not null)
        {
            var inputPath = GetTestFolder<T>(rootPath);
            if (Directory.Exists(inputPath))
            {
                foreach (var path in Directory.EnumerateFiles(inputPath, "*.txt"))
                {
                    yield return new TestCaseData(path).SetName($"{part}.{Path.GetFileNameWithoutExtension(path)}");
                }
            }
        }
    }

    public static void EnsureFolder(string path)
    {
        if (!Directory.Exists(path))
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
}
