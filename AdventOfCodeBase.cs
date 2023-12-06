using NUnit.Framework;
using System.Runtime.CompilerServices;

namespace AdventOfCode;

[TestFixture]
public abstract class AdventOfCodeBase<T>
{
    static IEnumerable<TestCaseData> AllInputs(string part) =>
        LocalInputs(part).Concat(PrivateInputs(part));

    static IEnumerable<TestCaseData> LocalInputs(string part) =>
        from path in Directory.EnumerateFiles(GetInputFolder(), "*.txt")
        select new TestCaseData(path).SetName($"{part}.{Path.GetFileNameWithoutExtension(path)}");

    static IEnumerable<TestCaseData> PrivateInputs(string part)
    {
        var rootPath = Environment.GetEnvironmentVariable("ADVENT_OF_CODE_INPUT_PATH");
        if (rootPath is not null)
        {
            var inputPath = Path.Combine(rootPath, GetTestSubfolder());
            if (Directory.Exists(inputPath))
            {
                foreach(var path in  Directory.EnumerateFiles(inputPath, "*.txt"))
                {
                    yield return new TestCaseData(path).SetName($"{part}.{Path.GetFileNameWithoutExtension(path)}");
                }
            }
        }

    }


    public virtual object? Solution1(string input) => default;

    public virtual object? Solution2(string input) => default;

    [TestCaseSource(nameof(AllInputs), ["Part1"])]
    public void Part1(string path)
    {
        var input = File.ReadAllText(path).Replace("\r\n", "\n");

        var result = Solution1(input);

        Approve(result, TestContext.CurrentContext.Test.Name);
    }

    [TestCaseSource(nameof(AllInputs), ["Part2"])]
    public void Part2(string path)
    {
        var input = File.ReadAllText(path).Replace("\r\n", "\n");

        var result = Solution2(input);

        Approve(result, TestContext.CurrentContext.Test.Name);
    }

    static string GetSlnFolder()
    {
        var currentDir = TestContext.CurrentContext.TestDirectory;
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

    static string GetTestSubfolder()
        => string.Join(
            Path.DirectorySeparatorChar,
            typeof(T).Namespace!
                .Split('.')
            .Skip(1)
            .Select(x => x.TrimStart('_'))
        );

    static string GetInputFolder()
        => Path.Combine(GetSlnFolder(), GetTestSubfolder(), "Input");

    string GetTestFolder()
    {
        var testNamespace = TestContext.CurrentContext.Test.Namespace;
        var subFolders = string.Join(Path.DirectorySeparatorChar,
            testNamespace!.Split('.').Skip(1).Select(x => x.TrimStart('_')));

        return Path.Combine(GetSlnFolder(), subFolders);
    }

    protected void Approve(string value, [CallerMemberName] string? callerMemberName = null)
    {
        var testFolder = GetTestFolder();

        var receivedFile = Path.Combine(testFolder, "Output", $"{callerMemberName}.received.txt");

        File.WriteAllText(receivedFile, value);

        var approvedFile = Path.Combine(testFolder, "Output", $"{callerMemberName}.approved.txt");

        if (!File.Exists(approvedFile))
        {
            File.WriteAllText(approvedFile, string.Empty);
        }

        var approvedText = File.ReadAllText(approvedFile);

        var normalizedApprovedText = approvedText.Replace("\r\n", "\n");
        var normalizedReceivedText = value.Replace("\r\n", "\n");

        if (!string.Equals(normalizedApprovedText, normalizedReceivedText))
        {
            throw new Exception("Approval verification failed.");
        }

        File.Delete(receivedFile);
    }

    protected void Approve(object? value, [CallerMemberName] string? callerMemberName = null)
    {
        Approve(value?.ToString() ?? "", callerMemberName);
    }
}

[AttributeUsage(AttributeTargets.Method)]
class SampleAttribute(string[] Samples) : Attribute
{

}
