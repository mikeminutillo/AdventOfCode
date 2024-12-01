using System.Collections.Immutable;

namespace AdventOfCode._2024._01;

public class Day01 : AdventOfCodeBase<Day01>
{
    public override object? Solution1(string input)
        => GetLists(input) switch
        {
            var (a, b) => Enumerable.Zip(
                a.Sort(), 
                b.Sort()
            ).Sum(x => Math.Abs(x.First - x.Second))
        };

    public override object? Solution2(string input)
        => GetLists(input) switch
        {
            var (a, b) => b.ToLookup(x => x) switch
            {
                var lookup => a.Sum(x => x * lookup[x].Count())
            }
        };


    static (ImmutableArray<long> a, ImmutableArray<long> b) GetLists(string input)
        => input
        .AsLines()
        .Select(x => x.ExtractLongNumbers().ToArray())
        .Aggregate(
            (
                a: ImmutableArray.Create<long>(),
                b: ImmutableArray.Create<long>()
            ), 
            (acc, val) => (
                [.. acc.a, val[0]],
                [.. acc.b, val[^1]]
            )
        );
}