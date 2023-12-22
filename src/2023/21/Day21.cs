using AdventOfCode.Infrastructure;
using System.Collections.Immutable;

namespace AdventOfCode._2023._21;

public class Day21 : AdventOfCodeBase<Day21>
{
    public override object? Solution1(string input)
        => Map.Parse(input) switch
        {
            var map => map.ReachableIn(64) switch
            {
                var path => map.Display(path).Count.Dump()
            }
        };

    public override object? Solution2(string input)
    => Map.Parse(input) switch
    {
        var map => map.MaxX != 130
            ? "SOLUTION DOES NOT WORK ON SAMPLE INPUT"
            : map.CalculateReachableFrom(26501365).Dump()
    };

    record Point(long X, long Y)
    {
        public static readonly Point Zero = new(0, 0);

        public ImmutableArray<Point> Adjacent =>
            [
                new(X, Y - 1),
                new(X, Y + 1),
                new(X - 1, Y),
                new(X + 1, Y),
            ];

        public Point TranslateTo(long maxX, long maxY)
            => new(
                ((X % (maxX + 1)) + maxX + 1) % (maxX + 1),
                ((Y % (maxY + 1)) + maxY + 1) % (maxY + 1)
                );

        public override string ToString()
            => $"({X}, {Y})";
    }

    record Map(int MaxX, int MaxY, ImmutableHashSet<Point> Rocks, Point Start)
    {
        public ImmutableHashSet<Point> Display(ImmutableHashSet<Point> path)
        {
            string.Join(Environment.NewLine,
                from y in Enumerable.Range(0, MaxY)
                select string.Join(null, from x in Enumerable.Range(0, MaxX)
                                         let p = new Point(x, y)
                                         let isPath = path.Contains(p)
                                         let isRocks = Rocks.Contains(p)
                                         select (isPath, isRocks) switch
                                         {
                                             (true, true) => '!',
                                             (true, false) => 'O',
                                             (false, true) => '#',
                                             (false, false) => '.'
                                         })
                ).Dump();

            return path;
        }

        public ImmutableHashSet<Point> ReachableIn(int steps)
            => Enumerable.Range(0, steps)
                .Aggregate(
                    ImmutableHashSet.Create(Start),
                    (state, _) => Reachable(state));

        ImmutableHashSet<Point> Reachable(ImmutableHashSet<Point> locations, bool infiniteMap = false)
            =>
            [
                .. from location in locations.AsParallel()
                   from adjacent in location.Adjacent
                   where infiniteMap || Contains(adjacent)
                   let adjusted = infiniteMap
                       ? adjacent.TranslateTo(MaxX, MaxY)
                       : adjacent
                   where Rocks.Contains(adjusted) == false
                   select adjacent
            ];

        ImmutableHashSet<Point> ReachableBorder(ImmutableHashSet<Point> inside, ImmutableHashSet<Point> border)
            => [.. from location in border.AsParallel()
                   from adjacent in location.Adjacent
                   where inside.Contains(adjacent) == false
                   let translated = adjacent.TranslateTo(MaxX, MaxY)
                   where Rocks.Contains(translated) == false
                   select adjacent];

        public long CalculateReachableFrom(int steps)
        {
            var border = new ImmutableHashSet<Point>[3];
            var reached = new ImmutableHashSet<Point>[3];

            // Prime the trackers
            var firstBorder = ReachableBorder([], [Start]);
            var secondBorder = ReachableBorder([], firstBorder);
            border[0] = firstBorder;
            reached[0] = firstBorder;
            border[1] = secondBorder;
            reached[1] = secondBorder;
            var step = 2;

            var runResults = new List<(int Steps, int Count)>();
            // NOTE: Assumes the cycle of steps reachable repeats after 3 rings
            // Which it does for the input
            for (var run = 0; run < 3; run++)
            {
                using var _ = Track.Time($"Run {run + 1}");
                // NOTE: Assumes a square map!
                // Which it is for the input
                var limit = run * (MaxX + 1) + (MaxX / 2);
                for (; step < limit; step++)
                {
                    // Keep track of the border and the reached separately
                    // Only explore the border
                    // We need to keep multiple sets to track odd and even
                    border[step % 3] = ReachableBorder(reached[(step - 2) % 3], border[(step - 1) % 3]);
                    reached[step % 3] = reached[(step - 2) % 3].Union(border[step % 3]);
                }
                runResults.Add((step, reached[(step - 1) % 3].Count));
                $"{step}: {reached[(step - 1) % 3].Count}".Dump();
            }

            return (long)LagrangeInterpolation(runResults.ToArray(), steps);
        }

        static double LagrangeInterpolation((int X, int Y)[] observations, int x)
            => (from i in Enumerable.Range(0, observations.Length)
                let term = Enumerable.Range(0, observations.Length)
                            .Aggregate(
                                (double)observations[i].Y,
                                (term, j) => j == i
                                    ? term
                                    : term * (x - observations[j].X)
                                            / (observations[i].X - observations[j].X))
                select term).Sum();

        bool Contains(Point p)
            => p.X >= 0 && p.X <= MaxX
            && p.Y >= 0 && p.Y <= MaxY;

        public static Map Parse(string input)
            => input.AsLines() switch
            {
                var lines => (from y in Enumerable.Range(0, lines.Length)
                              let line = lines[y]
                              from x in Enumerable.Range(0, line.Length)
                              let c = line[x]
                              select (x, y, c)
                              ).ToArray() switch
                {
                    var elements => new(
                        elements.Max(e => e.x),
                        elements.Max(e => e.y),
                        elements.Where(e => e.c == '#').Select(e => new Point(e.x, e.y)).ToImmutableHashSet(),
                        elements.Single(e => e.c == 'S') switch
                        {
                            var startElement => new Point(startElement.x, startElement.y)
                        })
                }
            };
    }
}