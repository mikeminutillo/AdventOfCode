using AdventOfCode.Infrastructure;
using System.Collections.Immutable;

namespace AdventOfCode._2023._13;

public class Day13 : AdventOfCodeBase<Day13>
{
    public override object? Solution1(string input)
        => Solve(input, 0);

    public override object? Solution2(string input)
        => Solve(input, 1);

    long Solve(string input, int allowedSmudges)
        => (from section in Section.Parse(input)
            let solve = section.Solve(allowedSmudges)
            //let _ = $"\n\n{section}\n\n".Dump()
            select solve
           ).Sum();

    record Section(string[] Map)
    {
        public static ImmutableArray<Section> Parse(string input)
            => input.Split("\n\n")
                .Select(x => x.AsLines())
                .Select(x => new Section(x))
                .ToImmutableArray();

        public ImmutableArray<char> Column(int x)
            => (from y in Enumerable.Range(0, Map.Length)
                select Map[y][x]).ToImmutableArray();

        public ImmutableArray<char> Row(int y)
            => [.. Map[y]];

        static int HowManySmudges(ImmutableArray<char> correct, ImmutableArray<char> smudged)
            => correct.SequenceEqual(smudged)
            ? 0
            : correct.AsSpan().CommonPrefixLength(smudged.AsSpan()) switch
            {
                var smudgePosition => 1 + HowManySmudges(correct, smudged.RemoveAt(smudgePosition).Insert(smudgePosition, correct[smudgePosition]))
            };


        static int GetSmudgesNeededForMirror(int lower, int upper, int length, Func<int, ImmutableArray<char>> getValues)
        {
            var totalSmudgeCount = 0;
            while (lower >= 0 && upper < length)
            {
                var correct = getValues(lower);
                var smudged = getValues(upper);
                totalSmudgeCount += HowManySmudges(correct, smudged);
                lower--;
                upper++;
            }

            return totalSmudgeCount;
        }

        public static int CheckMirror(int length, Func<int, ImmutableArray<char>> getValues, int smudges)
        {
            for (var i = 1; i < length; i++)
            {
                var x = GetSmudgesNeededForMirror(i - 1, i, length, getValues);
                if (x == smudges) return i;
            }

            return 0;
        }

        public int Solve(int smudges)
            => CheckMirror(Map[0].Length, Column, smudges) 
            + 100 * CheckMirror(Map.Length, Row, smudges);

        public override string ToString()
            => string.Join(Environment.NewLine, Map);
    }
}