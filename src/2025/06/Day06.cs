using System.Collections.Immutable;

namespace AdventOfCode._2025._06;

public class Day06 : AdventOfCodeBase<Day06>
{
    public override object? Solution1(string input)
        => input.AsLines()
            .Aggregate(
                ImmutableArray<ImmutableArray<decimal>>.Empty,
                (state, line) => line.ExtractNumbers<decimal>().ToImmutableArray() switch
                {
                    [] => [.. line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                            .Zip<string, ImmutableArray<decimal>, ImmutableArray<decimal>>(state, (op, nums) => op switch
                            {
                                "*" => [nums.Product()],
                                "+" => [nums.Sum()]
                            })],
                    var nums => state.Length < nums.Dump().Length
                        ? state.AddRange(nums.Select(num => ImmutableArray<decimal>.Empty.Add(num)))
                        : [.. state.Zip(nums, (acc, num) => acc.Add(num))]
                },
                state => state.SelectMany(x => x).Sum()
            );

    public override object? Solution2(string input)
        => Solve([.. input.AsLines()])
            .Sum();

    IEnumerable<decimal> Solve(string[] lines)
    {
        var lastLine = lines[^1];
        var numbers = lines[..^1];
        var lastIndex = 0;
        while (lastIndex >= 0)
        {
            var nextIndex = lastLine.IndexOfAny(['*', '+'], lastIndex + 1);
            var operands = ConvertToNumbers([.. numbers.Select(x => x[lastIndex..(nextIndex == -1 ? ^0 : (nextIndex - 1))])]);
            operands.Dump();
            yield return lastLine[lastIndex] switch
            {
                '+' => operands.Sum(),
                '*' => operands.Product(),
                var o => throw new Exception($"Unknown Operand {o}")
            };
            lastIndex = nextIndex;
        }

    }

    static IEnumerable<decimal> ConvertToNumbers(string[] numbers)
        => from idx in Enumerable.Range(0, numbers[0].Length)
           select numbers.Select(line => line[idx] switch
                {
                    ' ' => default(int?),
                    var c => c - '0'
                }).Where(x => x is not null)
                .Aggregate(0m, (acc, digit) => acc * 10m + digit!.Value, s => s)
           ;


}