using System.Collections.Immutable;

namespace AdventOfCode._2023._16;

public class Day16 : AdventOfCodeBase<Day16>
{
    const string North = "North";
    const string South = "South";
    const string East = "East";
    const string West = "West";

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
                        //let _ = $"{edge.entry}: {edge.direction}".Dump()
                        let lightBeam = map.TrackLightBeam(edge.entry, edge.direction)
                        select lightBeam.Count).Max().Dump()
        };

    static ImmutableDictionary<Point, ImmutableHashSet<string>> Display(Map map, ImmutableDictionary<Point, ImmutableHashSet<string>> light)
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
                             select new Component(line[x])).ToImmutableArray()
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

        public ImmutableDictionary<Point, ImmutableHashSet<string>> TrackLightBeam(Point start, string direction)
        {
            var visited = new Dictionary<Point, HashSet<string>>();

            var toVisit = new Queue<(Point location, string direction)>();
            toVisit.Enqueue((start, direction));

            while(toVisit.TryDequeue(out var current))
            {
                if(!visited.TryGetValue(current.location, out var visitedDirections))
                {
                    visitedDirections = [];
                    visited.Add(current.location, visitedDirections);
                }

                if(visitedDirections.Contains(current.direction))
                {
                    continue;
                }

                visitedDirections.Add(current.direction);

                var component = At(current.location);

                foreach(var newDirection in component.AffectLight(current.direction))
                {
                    var newLocation = current.location.Move(newDirection);
                    if (Contains(newLocation))
                    {
                        toVisit.Enqueue((newLocation, newDirection));
                    }
                }
            }

            return visited.ToImmutableDictionary(x => x.Key, x => x.Value.ToImmutableHashSet());
        }

        public IEnumerable<(Point entry, string direction)> GetEdges()
        {
            foreach(var x in Enumerable.Range(0, MaxX + 1))
            {
                yield return (new Point(x, 0), South);
                yield return (new Point(x, MaxY), North);
            }

            foreach(var y in Enumerable.Range(0, MaxY + 1))
            {
                yield return (new Point(0, y), East);
                yield return (new Point(MaxX, y), West);
            }
        }
    }

    record Component(char Symbol)
    {
        public ImmutableArray<string> AffectLight(string entry)
            => Symbol switch
            {
                '.' => [entry],
                '/' => entry switch
                {
                    North => [East],
                    South => [West],
                    East => [North],
                    West => [South],
                    var x => throw new Exception($"Unknown direction {x}")
                },
                '\\' => entry switch
                {
                    North => [West],
                    South => [East],
                    East => [South],
                    West => [North],
                    var x => throw new Exception($"Unknown direction {x}")
                },
                '-' => entry switch
                {
                    East or West => [entry],
                    North or South => [East, West],
                    var x => throw new Exception($"Unknown direction {x}")
                },
                '|' => entry switch
                {
                    East or West => [North, South],
                    North or South => [entry],
                    var x => throw new Exception($"Unknown direction {x}")
                },
                var x => throw new Exception($"Unknown component {x}")
            };

        public override string ToString() => $"{Symbol}";
    }

    record Point(int X, int Y)
    {
        public Point Move(string direction)
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