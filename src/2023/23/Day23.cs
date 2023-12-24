using System.Collections.Immutable;

namespace AdventOfCode._2023._23;

public class Day23 : AdventOfCodeBase<Day23>
{
    public override object? Solution1(string input)
        => Map.Parse(input) switch
        {
            var map => map.AsGraph() switch
            {
                var graph => graph.LongestPathLength(map.GetStart(), map.GetEnd()) switch
                {
                    var longestPath => longestPath.Dump()
                }
            }
        };

    public override object? Solution2(string input)
        => Map.Parse(input) switch
        {
            var map => map.WithoutSlopes().AsGraph() switch
            {
                var graph => graph.LongestPathLength(map.GetStart(), map.GetEnd()) switch
                {
                    var longestPath => longestPath.Dump()
                }
            }
        };

    record Point(int X, int Y)
    {
        public Point Left => new(X - 1, Y);
        public Point Right => new(X + 1, Y);
        public Point Up => new(X, Y - 1);
        public Point Down => new(X, Y + 1);

        public ImmutableArray<Point> GetAdjacent()
            => [Up, Down, Left, Right];

        public override string ToString()
            => $"({X}, {Y})";
    }

    record Map(ImmutableDictionary<Point, char> Elements)
    {
        bool IsFree(Point p)
            => Elements.ContainsKey(p) && Elements[p] != '#';

        bool IsRoad(Point p)
            => p.GetAdjacent().Count(IsFree) == 2;

        public Graph AsGraph()
            => (from point in Elements.Keys
                where IsFree(point)
                where !IsRoad(point)
                select point
               ).ToImmutableArray() switch
            {
                var crossRoads => (from i in Enumerable.Range(0, crossRoads.Length).AsParallel()
                                   from j in Enumerable.Range(0, crossRoads.Length)
                                   where i != j
                                   let @from = crossRoads[i]
                                   let @to = crossRoads[j]
                                   let distance = Distance(@from, @to)
                                   where distance > 0
                                   select new Edge(@from, to, distance)
                        ).ToImmutableArray() switch
                {
                    var edges => new(edges)
                }
            };

        ImmutableArray<Point> GetExits(Point current)
            => Elements[current] switch
            {
                '.' => [current.Left, current.Right, current.Up, current.Down],
                '>' => [current.Right],
                '<' => [current.Left],
                '^' => [current.Up],
                'v' => [current.Down],
                '#' => [],
                var c => throw new Exception($"Unexpected map element {c}")
            };

        int Distance(Point from, Point to)
        {
            var q = new Queue<(Point Current, int Distance)>();
            var visited = new HashSet<Point>();
            q.Enqueue((from, 0));
            while(q.Count != 0)
            {
                var (current, distance) = q.Dequeue();

                foreach (var adjacent in GetExits(current))
                {
                    if(adjacent == to)
                    {
                        return distance + 1;
                    }
                    else if(IsRoad(adjacent) && !visited.Contains(adjacent))
                    {
                        visited.Add(adjacent);
                        q.Enqueue((adjacent, distance + 1));
                    }
                }
            }

            return -1;
        }

        public Map WithoutSlopes()
            => new(Elements.Select(x => x.Value is '<' or '>' or 'v' or '^'
                ? new KeyValuePair<Point, char>(x.Key, '.')
                : x
                ).ToImmutableDictionary());

        public Point GetStart()
            => Elements.Where(e => e.Value == '.').OrderBy(e => e.Key.Y).First().Key;


        public Point GetEnd()
            => Elements.Where(e => e.Value == '.').OrderBy(e => e.Key.Y).Last().Key;

        public static Map Parse(string input)
            => new(
                input.AsLines() switch
                {
                    var lines => (from y in Enumerable.Range(0, lines.Length)
                                  let line = lines[y]
                                  from x in Enumerable.Range(0, line.Length)
                                  let c = line[x]
                                  select (Point: new Point(x, y), Char: c))
                    .ToImmutableDictionary(
                        x => x.Point,
                        x => x.Char
                        )
                });
    }

    record Graph(ImmutableArray<Edge> Edges)
    {
        readonly ImmutableDictionary<Point, long> nodeIndex = Edges.Select(e => e.From)
                .Union(Edges.Select(e => e.To))
                .Distinct()
                .Select((p, i) => (p, idx: 1L << (i - 1)))
                .ToImmutableDictionary(
                    d => d.p,
                    d => d.idx
                );

        bool HasVisited(Point p, long track)
            => (track & nodeIndex[p]) != 0;

        long Visit(Point p, long track)
            => track | nodeIndex[p];

        public int LongestPathLength(Point start, Point end)
        {
            var cache = new Dictionary<(Point, long), int>();

            int LongestPath(Point point, long visited)
            {
                if(point == end)
                {
                    return 0;
                } 
                else if(HasVisited(point, visited))
                {
                    return int.MinValue;
                }
                var key = (point, visited);
                if (!cache.TryGetValue(key, out int value))
                {
                    value = Edges
                        .Where(e => e.From == point)
                        .Select(e => e.Cost + LongestPath(e.To, Visit(point, visited)))
                        .Max();
                    cache[key] = value;
                }
                return value;
            }

            return LongestPath(start, 0);
        }
    }

    record Edge(Point From, Point To, int Cost);
}