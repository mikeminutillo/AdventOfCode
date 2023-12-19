namespace AdventOfCode._2023._18;

using System.Collections.Immutable;
using System.Globalization;
using static Direction;

enum Direction
{
    U, D, L, R
}

public class Day18 : AdventOfCodeBase<Day18>
{
    public override object? Solution1(string input) 
        => input.AsLines()
        .Select(Instruction.Parse)
        .ToImmutableArray() switch
        {
            var instructions => Solve(instructions).Dump()
        };

    public override object? Solution2(string input)
        => input.AsLines()
        .Select(Instruction.Parse)
        .Select(i => i.Transform)
        .ToImmutableArray() switch
        {
            var instructions => Solve(instructions).Dump()
        };

    static long Solve(ImmutableArray<Instruction> instructions)
        => PitSize(new Point(0, 0) switch
        {
            var origin => instructions.Aggregate(
                (Location: origin, Pit: ImmutableArray.Create(origin)),
                (state, instruction) => instruction.Apply(state.Location) switch
                {
                    var newLocation => (newLocation, state.Pit.Add(newLocation))
                }).Pit
        }, instructions.Sum(i => i.Distance));

    static long PitSize(ImmutableArray<Point> vertices, long boundary)
        => ShoelaceFormula(vertices) switch
        {
            var area => (area - boundary / 2 + 1) switch
            {
                var interior => boundary + interior
            }
        };

    static long ShoelaceFormula(ImmutableArray<Point> vertices) 
        => Math.Abs(
            Enumerable.Zip(vertices, vertices[1..].Add(vertices[0]))
                      .Select(a => a.First.X * a.Second.Y - a.First.Y * a.Second.X)
                      .Sum()
            ) / 2;

    record Point(long X, long Y);

    record Instruction(Direction Direction, int Distance, string Color)
    {
        public static Instruction Parse(string line)
            => Regex.Match(line, @"(U|D|L|R) (\d+) \((.*)\)") is { Success: true } match
            ? new(Enum.Parse<Direction>(match.Result("$1")), int.Parse(match.Result("$2")), match.Result("$3"))
            : throw new Exception($"Cannot parse {line}");

        public Point Apply(Point location)
            => Direction switch
            {
                U => new(location.X, location.Y - Distance),
                D => new(location.X, location.Y + Distance),
                L => new(location.X - Distance, location.Y),
                R => new(location.X + Distance, location.Y)
            };

        public Instruction Transform
            => Regex.Match(Color, @"#(.{5})(.)") is { Success: true } match
            ? new(match.Result("$2") switch
            {
                "0" => R,
                "1" => D,
                "2" => L,
                "3" => U,
                var x => throw new Exception($"Badly formed color {Color} does not end with 0-3")
            }, int.Parse(match.Result("$1"), NumberStyles.HexNumber), "TRANSFORMED")
            : throw new Exception($"Badly encoded color {Color} could not be parsed");
    }
}