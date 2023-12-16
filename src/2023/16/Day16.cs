using System.Collections.Immutable;

namespace AdventOfCode._2023._16;

using static Direction;

enum Direction
{
    North,
    South,
    East,
    West
}

public class Day16 : AdventOfCodeBase<Day16>
{
    //const string North = "North";
    //const string South = "South";
    //const string East = "East";
    //const string West = "West";


    public override object? Solution1(string input)
        => Map.Parse(input) switch
        {
            var map => map.TrackLightBeam(new(0, 0), East) switch
            {
                var light => Display(map, light)
                                .Count
                                .Dump()
            }
        };

    public override object? Solution2(string input)
    => Map.Parse(input) switch
    {
        var map => (from edge in map.GetEdges().AsParallel()
                    select map.TrackLightBeam(edge.entry, edge.direction)
                                .Count
                    ).Max().Dump()
    };

    static Dictionary<Point, HashSet<Direction>> Display(Map map, Dictionary<Point, HashSet<Direction>> light)
    {
        string.Join(Environment.NewLine,
            from y in Enumerable.Range(0, map.Components.Length)
            select string.Join(null,
                from x in Enumerable.Range(0, map.Components[y].Length)
                let point = new Point(x, y)
                let component = map.At(point)
                select component.Symbol is '.'
                ? light.TryGetValue(point, out var result)
                    ? "#"
                    : $"{component}"
                : $"{component}"
                )
            ).Dump();

        return light;
    }

    record Map(ImmutableArray<ImmutableArray<Component>> Components)
    {
        public static Map Parse(string input)
            => input.AsLines() switch
            {
                var lines => new(
                    (from y in Enumerable.Range(0, lines.Length)
                     let line = lines[y]
                     select (from x in Enumerable.Range(0, line.Length)
                             select Component.Build(line[x])).ToImmutableArray()
                     ).ToImmutableArray()
                )
            };

        public int MaxX => Components[0].Length - 1;
        public int MaxY => Components.Length - 1;

        public bool Contains(Point p)
            => p.X >= 0 && p.X <= MaxX
            && p.Y >= 0 && p.Y <= MaxY;

        public override string ToString()
            => string.Join(
                Environment.NewLine,
                from line in Components
                select string.Join(null, line)
            );

        public Component At(Point p)
            => Components[p.Y][p.X];

        public Dictionary<Point, HashSet<Direction>> TrackLightBeam(Point start, Direction direction)
        {
            var visited = new Dictionary<Point, HashSet<Direction>>();

            var toVisit = new Queue<(Point location, Direction direction)>();
            toVisit.Enqueue((start, direction));

            while (toVisit.TryDequeue(out var current))
            {
                if (!visited.TryGetValue(current.location, out var visitedDirections))
                {
                    visitedDirections = [];
                    visited.Add(current.location, visitedDirections);
                }
                else if (visitedDirections.Contains(current.direction))
                {
                    continue;
                }

                visitedDirections.Add(current.direction);

                var component = At(current.location);

                foreach (var newDirection in component.AffectLight(current.direction))
                {
                    var newLocation = current.location.Move(newDirection);
                    if (Contains(newLocation))
                    {
                        toVisit.Enqueue((newLocation, newDirection));
                    }
                }
            }

            return visited;
        }

        public IEnumerable<(Point entry, Direction direction)> GetEdges()
        {
            foreach (var x in Enumerable.Range(0, MaxX + 1))
            {
                yield return (new Point(x, 0), South);
                yield return (new Point(x, MaxY), North);
            }

            foreach (var y in Enumerable.Range(0, MaxY + 1))
            {
                yield return (new Point(0, y), East);
                yield return (new Point(MaxX, y), West);
            }
        }
    }

    record Component(char Symbol, Dictionary<Direction, Direction[]> Operations)
    {
        public Direction[] AffectLight(Direction entry)
            => Operations.TryGetValue(entry, out var newDirections)
            ? newDirections
            : [entry];

        public static Component Build(char c)
            => library[c];

        static readonly Dictionary<char, Component> library = new()
        {
            ['.'] = new('.', new Dictionary<Direction, Direction[]>
            {
                [North] = [North],
                [South] = [South],
                [West] = [West],
                [East] = [East]
            }),
            ['/'] = new('/', new Dictionary<Direction, Direction[]>
            {
                [North] = [East],
                [South] = [West],
                [West] = [South],
                [East] = [North]
            }),
            ['\\'] = new('\\', new Dictionary<Direction, Direction[]>
            {
                [North] = [West],
                [South] = [East],
                [West] = [North],
                [East] = [South]
            }),
            ['-'] = new('-', new Dictionary<Direction, Direction[]>
            {
                [North] = [East, West],
                [South] = [East, West],
                [West] = [West],
                [East] = [East]
            }),
            ['|'] = new('|', new Dictionary<Direction, Direction[]>
            {
                [North] = [North],
                [South] = [South],
                [West] = [North, South],
                [East] = [North, South]
            })
        };

        public override string ToString() => $"{Symbol}";
    }

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

        public override string ToString()
            => $"({X}, {Y})";
    }
}