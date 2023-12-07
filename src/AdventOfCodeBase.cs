﻿using System.Runtime.CompilerServices;

namespace AdventOfCode;

[TestFixture]
public abstract class AdventOfCodeBase<T>
{
    static IEnumerable<TestCaseData> AllInputs(string part) =>
        LocalInputs(part).Concat(PrivateInputs(part));

    static IEnumerable<TestCaseData> LocalInputs(string part) =>
        from path in Directory.EnumerateFiles(
            Path.Combine(Extensions.GetTestFolder<T>(), "Input"), 
            "*.txt")
        select new TestCaseData(path).SetName($"{part}.{Path.GetFileNameWithoutExtension(path)}");

    static IEnumerable<TestCaseData> PrivateInputs(string part)
    {
        var rootPath = Environment.GetEnvironmentVariable("ADVENT_OF_CODE_INPUT_PATH");
        if (rootPath is not null)
        {
            var inputPath = Extensions.GetTestFolder<T>(rootPath);
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

    protected void Approve(object? value, [CallerMemberName] string? callerMemberName = null)
    {
        Approvals.Approve(value?.ToString() ?? "", Path.Combine(Extensions.GetTestFolder<T>(), "Output"), callerMemberName);
    }
}