namespace AdventOfCode._2023._06;

public class Day06 : AdventOfCodeBase<Day06>
{
    public override object? Solution1(string input)
        => GetRaces(input).Product(x => x.WaysToWin);

    public override object? Solution2(string input)
        => ParseRaceWithFixedKerning(input.AsLines())
            .WaysToWin;

    static IEnumerable<Race> GetRaces(string input)
        => ParseRaces(input.AsLines());

    static IEnumerable<Race> ParseRaces(string[] lines)
        => from pair in Enumerable.Zip(lines[0].ExtractNumbers(), lines[1].ExtractNumbers())
           select new Race(pair.First, pair.Second);

    static Race ParseRaceWithFixedKerning(string[] lines)
        => new Race(
                ExtractLongNumber(lines[0]),
                ExtractLongNumber(lines[1])
            );

    static long ExtractLongNumber(string input)
        => long.Parse(new string(input.Where(char.IsDigit).ToArray()));


    record Race(long Time, long Distance)
    {
        // X: Time to hold button
        // Speed = X
        // TimeRemaining = Time - X
        // DistanceTravelled = TimeRemaining * Speed
        // DistanceTravelled > Distance
        // (Time - X) * X - Distance > 0
        // Solve Quadratic Equation https://en.wikipedia.org/wiki/Quadratic_equation
        public int WaysToWin
        {
            get
            {
                var a = -1;
                var b = Time;
                var c = -Distance;

                var sqrt = Math.Sqrt(b * b - (4 * a * c));
                // Ceiling and floor because we need integers that beat the Distance
                var firstBoundary = (int)Math.Floor((-b + sqrt) / (2 * a));
                var secondBoundary = (int)Math.Ceiling((-b - sqrt) / (2 * a))-1;
                var solutions = secondBoundary - firstBoundary;

                $"({Time}, {Distance}): {solutions} solutions [{firstBoundary}..{secondBoundary}]".Dump();
                return solutions;
            }
        }
    }
}
