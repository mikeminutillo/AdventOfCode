namespace AdventOfCode.Infrastructure;

static class Utility
{
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
