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
            let vm = section.VerticalMirror(allowedSmudges)
            let hm = section.HorizontalMirror(allowedSmudges)
            select vm + 100 * hm
           ).Sum();

    [TestCase(ExpectedResult =0)]
    public int Foo()
        => Section.Parse(File.ReadAllText(Path.Combine(Utility.GetTestFolder<Day13>(), "Input", "sample.txt")))
            .First() switch
            {
                var section => Section.CheckMirror(0, 6, section.Row, 1).remainingSmudges
            };


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
            => Map[y].ToImmutableArray();

        public static ImmutableArray<char> FixNextSmudge(ImmutableArray<char> correct, ImmutableArray<char> smudged)
            => correct.AsSpan().CommonPrefixLength(smudged.AsSpan()) switch
            {
                var smudgePosition => smudged.RemoveAt(smudgePosition).Insert(smudgePosition, correct[smudgePosition])
            };


       public static (bool isMirror, int remainingSmudges) CheckSections(ImmutableArray<char> a, ImmutableArray<char> b, int allowedSmudges)
            => a.SequenceEqual(b) switch
            {
                true => (true, allowedSmudges),
                false => allowedSmudges > 0
                            ? CheckSections(a, FixNextSmudge(a, b), allowedSmudges - 1)
                            : (false, allowedSmudges)
            };

        public static (bool isMirror, int remainingSmudges) CheckMirror(int a, int b, Func<int, ImmutableArray<char>> getValues, int requiredSmudges)
            => //(a, b) == (a, b).Dump()
            requiredSmudges < 0
            ? (false, requiredSmudges)
            : b <= a
            ? (true, requiredSmudges)
            //: getValues(a).SequenceEqual(getValues(b))
            : CheckSections(getValues(a), getValues(b), requiredSmudges) switch
            {
                (true, var remainingSmudges) => CheckMirror(a + 1, b - 1, getValues, remainingSmudges),
                (false, var remainingSmudges) => (false, remainingSmudges)
            };
            //: (false, requiredSmudges);

        static int CheckMirror(int length, Func<int, ImmutableArray<char>> getValues, int requiredSmudges)
            => (from smudge in Enumerable.Range(0, requiredSmudges + 1).Reverse()
                from x in Enumerable.Range(0, length)
                from offset in Enumerable.Range(0, 2) // [0, 1]
                let width = Math.Min(x, length - (x + offset + 1))
                where width + offset > 0
                let left = x - width
                let right = x + width + offset
                let isMirror = CheckMirror(left, right, getValues, smudge)
                let _ = $"Mirror of width {width} centered on {x} with offset {offset} from {left} to {right}: {isMirror}".Dump()
                where isMirror.isMirror && isMirror.remainingSmudges == 0
                select x + offset
                ).FirstOrDefault();

        public int VerticalMirror(int requiredSmudges = 0)
            => CheckMirror(Map[0].Length, Column, requiredSmudges);

        public int HorizontalMirror(int requiredSmudges = 0)
            => CheckMirror(Map.Length, Row, requiredSmudges);

        public override string ToString()
            => string.Join(Environment.NewLine, Map);
    }
}