using System.Collections.Immutable;

namespace AdventOfCode._2024._02;

public class Day02 : AdventOfCodeBase<Day02>
{
    public override object? Solution1(string input)
        => input.AsLines()
                .Select(Report.Parse)
                .Count(x => x.IsSafe);

    public override object? Solution2(string input)
        => input.AsLines()
                .Select(Report.Parse)
                .Count(x => x.IsSafeWithDampener);

    record Report(ImmutableArray<int> Levels)
    {
        public static Report Parse(string text)
            => new(text.ExtractNumbers().ToImmutableArray());

        public bool IsSafe =>
            Enumerable.Zip(Levels[..^1], Levels[1..])
                      .Select(x => x.First - x.Second)
                      .ToArray() switch
                      {
                          var diffs => diffs.Select(Math.Sign).Distinct().Count() == 1
                                    && diffs.Select(Math.Abs).All(x => x is > 0 and <= 3)
                      };

        public bool IsSafeWithDampener =>
            IsSafe || Enumerable.Range(0, Levels.Length)
                                .Select(idx => this with
                                {
                                    Levels = Levels.RemoveAt(idx)
                                }).Any(x => x.IsSafe);
    }
}