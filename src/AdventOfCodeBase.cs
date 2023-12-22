using System.Runtime.CompilerServices;
using AdventOfCode.Infrastructure;

namespace AdventOfCode;

[TestFixture]
public abstract class AdventOfCodeBase<T>
{
    static IEnumerable<TestCaseData> AllInputs(string part)
        => Utility.AllInputs<T>(part);

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

    protected void Approve(object? value, [CallerMemberName] string? callerMemberName = null)
    {
        Approvals.Approve(value?.ToString() ?? "", Path.Combine(Utility.GetTestFolder<T>(), "Output"), callerMemberName);
    }
}