using NUnit.Framework;
using System.Runtime.CompilerServices;

namespace AdventOfCode;

[TestFixture]
public abstract class AdventOfCodeBase
{
    protected string Load(string path) => File.ReadAllText(
          Path.Combine(GetTestFolder(), "Input", $"{path}.txt")
        );

    protected string GetFile(string path) => Path.Combine(GetTestFolder(), path);

    public virtual object? Solution1(string input) => default;

    public virtual object? Solution2(string input) => default;

    [TestCase]
    public void Problem1()
    {
        var input = Load("input");

        var result = Solution1(input);

        if(result is not null)
            Approve(result);
    }

    [TestCase]
    public void Problem2()
    {
        var input = Load("input");

        var result = Solution2(input);

        if (result is not null)
            Approve(result);
    }

    string GetSlnFolder()
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

    protected void Approve(object value, [CallerMemberName] string? callerMemberName = null)
    {
        Approve(value?.ToString() ?? "", callerMemberName);
    }
}
