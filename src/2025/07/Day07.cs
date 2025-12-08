using System.Collections.Immutable;
using System.Linq;

namespace AdventOfCode._2025._07;

public class Day07 : AdventOfCodeBase<Day07>
{
    public override object? Solution1(string input)
        => input.AsLines()
            .Aggregate(
                (Beams: ImmutableHashSet<int>.Empty, Splits: 0),
                (state, line) => line.IndexOf('S') switch
                {
                    -1 => Enumerable.Range(0, line.Length)
                        .Where(i => line[i] == '^')
                        .Intersect(state.Beams)
                        .ToImmutableArray() switch
                        {
                            var activatedSplitters => (
                                Beams: state.Beams.Except(activatedSplitters)
                                    .Union(from s in activatedSplitters select s - 1)
                                    .Union(from s in activatedSplitters select s + 1),
                                Splits: state.Splits + activatedSplitters.Length
                            )
                        },
                    var start => state with { Beams = [start] }
                },
                state => state.Splits
            );

    public override object? Solution2(string input)
        => input.AsLines()
            .Aggregate(
                ImmutableArray<decimal>.Empty,
                (timelines, line) => line.IndexOf('S') switch
                {
                    -1 => [
                            .. from i in Enumerable.Range(0, timelines.Length)
                               let left = (i > 0 && line[i - 1] == '^') ? timelines[i - 1] : 0
                               let above = line[i] == '^' ? 0 : timelines[i]
                               let right = (i < timelines.Length - 1 && line[i + 1] == '^') ? timelines[i + 1] : 0
                               select (left + above + right)
                          ],
                    var start => Enumerable.Repeat(0m, line.Length).ToImmutableArray().SetItem(start, 1)
                },
                state => state.Sum()
            );
}