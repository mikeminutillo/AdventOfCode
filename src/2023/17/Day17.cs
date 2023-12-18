using System.Collections.Immutable;

namespace AdventOfCode._2023._17;

using static Direction;

enum Direction
{
    North,
    South,
    East,
    West
}

public class Day17 : AdventOfCodeBase<Day17>
{
    public override object? Solution1(string input)
        => HeatLossMap.Parse(input) switch
        {
            var map => map.ShortestPath(
                new Point(0, 0),
                new Point(map.MaxX, map.MaxY),
                0, 
                3) switch
            {
                var path => map.Cost(Display(path, map)).Dump()
            }
        };

    public override object? Solution2(string input)
        => HeatLossMap.Parse(input) switch
        {
            var map => map.ShortestPath(
                new Point(0, 0),
                new Point(map.MaxX, map.MaxY),
                4,
                10) switch
            {
                var path => map.Cost(Display(path, map)).Dump()
            }
        };

    static ImmutableArray<(Point location, Direction direction)> Display(ImmutableArray<(Point location, Direction direction)> path, HeatLossMap map)
    {
        string.Join(Environment.NewLine, from y in Enumerable.Range(0, map.MaxY + 1)
                                         select new string(
                                             (from x in Enumerable.Range(0, map.MaxX + 1)
                                              let point = new Point(x, y)
                                              select path.Any(x => x.location == point) switch
                                              {
                                                  false => (char)('0' + map.HeatLossAt(point)),
                                                  true => path.First(x => x.location == point).direction switch
                                                  {
                                                      North => '^',
                                                      South => 'v',
                                                      East => '>',
                                                      West => '<'
                                                  }

                                              }).ToArray()
                                            )).Dump();

        return path;
    }

    static Direction OppositeDirection(Direction direction)
        => direction switch
        {
            North => South,
            South => North,
            East => West,
            West => East,
            var x => throw new Exception($"Unknown direction {x}")
        };

    readonly static ImmutableArray<Direction> AllDirections = [North, South, East, West];

    record Point(int X, int Y)
    {
        public Point Move(Direction direction)
            => direction switch
            {
                North => new(X, Y - 1),
                South => new(X, Y + 1),
                East => new(X + 1, Y),
                West => new(X - 1, Y),
                var x => throw new Exception($"Unknown direction {x}")
            };

        public int DistanceTo(Point p)
            => Math.Abs(p.X - X)
            + Math.Abs(p.Y - Y);

        public override string ToString()
            => $"({X}, {Y})";
    }


    record HeatLossMap(ImmutableArray<ImmutableArray<int>> Entries)
    {
        public static HeatLossMap Parse(string input)
            => new(
                input.AsLines()
                     .Select(line => line.Select(c => c - '0')
                                         .ToImmutableArray()
                ).ToImmutableArray()
            );

        public int MaxX => Entries[0].Length - 1;
        public int MaxY => Entries.Length - 1;

        public bool Contains(Point p)
            => p.X >= 0 && p.X <= MaxX
            && p.Y >= 0 && p.Y <= MaxY;

        public int HeatLossAt(Point p)
            => Entries[p.Y][p.X];

        public int Cost(IEnumerable<(Point location, Direction direction)> points)
            => points.Sum(p => HeatLossAt(p.location));

        public ImmutableArray<(Point location, Direction direction)> ShortestPath(Point start, Point end, int minStraightLine, int maxStraightLine)
        {
            var q = new PriorityQueue<PathStep, int>();
            q.Enqueue(PathStep.Start(start, East, 0), 0);
            q.Enqueue(PathStep.Start(start, South, 0), 0);

            var beenThere = new HashSet<(Point location, Direction direction, int straightLineCount)>();

            while(q.TryDequeue(out var step, out var totalCost))
            {
                if(step.Location == end && step.StraightLineCount >= minStraightLine)
                {
                    return [.. step.Steps()];
                }

                var nextSteps = from direction in step.ValidDirections(minStraightLine, maxStraightLine)
                                let newLocation = step.Location.Move(direction)
                                let nextStep = step.AddStep(newLocation, direction)
                                select nextStep;

                foreach(var nextStep in nextSteps)
                {
                    if(Contains(nextStep.Location) 
                    && !beenThere.Contains((nextStep.Location, nextStep.Direction, nextStep.StraightLineCount)))
                    {
                        beenThere.Add((nextStep.Location, nextStep.Direction, nextStep.StraightLineCount));
                        q.Enqueue(nextStep, totalCost + HeatLossAt(nextStep.Location));
                    }
                }
            }

            throw new Exception($"No path from {start} to {end}");
        }

        record PathStep(Point Location, Direction Direction, int StraightLineCount, PathStep? PreviousStep)
        {
            public PathStep AddStep(Point newLocation, Direction newDirection)
                => new(
                    newLocation,
                    newDirection,
                    newDirection == Direction
                        ? StraightLineCount + 1
                        : 1,
                    this
                    );

            public IEnumerable<Direction> ValidDirections(int minStraightLineCount, int maxStraightLineCount)
            {
                var oppositeDirection = OppositeDirection(Direction);
                if(StraightLineCount < minStraightLineCount)
                {
                    yield return Direction;
                }
                else if(StraightLineCount < maxStraightLineCount)
                {
                    foreach(var direction in AllDirections.Where(d => d != oppositeDirection))
                    {
                        yield return direction;
                    }
                }
                else
                {
                    foreach(var direction in AllDirections.Where(d => d != oppositeDirection && d != Direction))
                    {
                        yield return direction;
                    }
                }
            }

            public static PathStep Start(Point location, Direction direction, int straightLineCount)
                => new(location, direction, straightLineCount, null);

            public ImmutableArray<(Point location, Direction direction)> Steps()
                => PreviousStep switch
                {
                    null => [],
                    var prev => [.. prev.Steps(), new(Location, Direction)]
                };

            public override string ToString()
                => $"{StraightLineCount} {Direction} -> {Location}" ;
        }
    }
}