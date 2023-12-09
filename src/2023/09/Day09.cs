using System.Collections.Immutable;

namespace AdventOfCode._2023._09;

public class Day09 : AdventOfCodeBase<Day09>
{
    public override object? Solution1(string input)
        => (from numbers in Parse(input)
            select GetNextNumber(numbers.ToImmutableArray())
           ).Sum();

    public override object? Solution2(string input)
        => (from numbers in Parse(input)
            select GetNextNumber(numbers.Reverse().ToImmutableArray())
           ).Sum();

    static IEnumerable<IEnumerable<int>> Parse(string input)
        => from line in input.AsLines()
           select Regex.Matches(line, @"(-?\d+)")
                       .Select(x => x.Value)
                       .Select(int.Parse);

    static int GetNextNumber(ImmutableArray<int> input)
        => input.All(x => x == 0) ? 0
        : input[^1] + GetNextNumber(GetDifferences(input));

    static ImmutableArray<int> GetDifferences(ImmutableArray<int> input)
        => (from i in Enumerable.Range(0, input.Length - 1)
            select input[i + 1] - input[i])
        .ToImmutableArray();
}