using System.Collections.Immutable;

namespace AdventOfCode._2024._01;

public partial class Day01 : AdventOfCodeBase<Day01>
{

    public override object? Solution1(string input)
        => GetLists(input.AsLines()) switch
        {
            var (a, b) => a.Sort()
                           .Zip(b.Sort())
                           .Sum(x => Math.Abs(x.First - x.Second))
        };

    public override object? Solution2(string input)
        => GetLists(input.AsLines()) switch
        {
            var (a, b) => b.ToLookup(x => x) switch
            {
                var lookup => a.Sum(x => x * lookup[x].Count())
            }
        };


    static (ImmutableArray<long> a, ImmutableArray<long> b) GetLists(string[] lines)
        => lines.Select(x => Numbers().Matches(x).Select(x => long.Parse(x.Value)))
        .Aggregate(
            (
                a: ImmutableArray.Create<long>(),
                b: ImmutableArray.Create<long>()
            ), 
            (acc, val) => (
                acc.a.Add(val.First()),
                acc.b.Add(val.Last())
            )
        );

    

    [GeneratedRegex(@"\d+")]
    private static partial Regex Numbers();
}