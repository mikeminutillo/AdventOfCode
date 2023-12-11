namespace AdventOfCode._2023._10;

using System.Collections.Immutable;
using Map = string[];
using Loop = HashSet<Day10.Point>;

public class Day10 : AdventOfCodeBase<Day10>
{
    public override object? Solution1(string input)
        => FindLoop(input.AsLines()).Count / 2;

    public override object? Solution2(string input)
        => string.Join(Environment.NewLine,
            MapInside(input.AsLines())
        ).Dump().Where(c => c == '*').Count();

    static Point FindStart(Map map)
        => (from row in Enumerable.Range(0, map.Length)
            let sPosition = map[row].IndexOf('S')
            where sPosition > -1
            select new Point(sPosition, row)).Single();

    static Map MapInside(Map map)
        => MapInsideClean(map, FindLoop(map));

    static Map MapInsideClean(Map map, HashSet<Point> loop)
        => MapInside(SimplifiedMap(map, loop), loop);

    static Map MapInside(Map map, HashSet<Point> loop)
        => (from y in Enumerable.Range(0, map.Length)
           select new string((from x in Enumerable.Range(0, map[y].Length)
                              let point = new Point(x, y)
                              select IsInsideLoop(map, loop, point)
                                ? '*'
                                : map[y][x]).ToArray())).ToArray();

    static bool IsInsideLoop(Map map, HashSet<Point> loop, Point point)
        => !loop.Contains(point)
        && Enumerable.Range(1, Math.Max(point.X - 1, 0))
            .Aggregate(
                false, 
                (inside, offset) => new Point(point.X - offset, point.Y).RequiresJumping(map, loop)
                    ? !inside
                    : inside);

    static HashSet<Point> FindLoop(Map map)
    {
        var visited = new HashSet<Point>();
        var queue = new Queue<Point>();
        queue.Enqueue(FindStart(map));
        while(queue.TryDequeue(out var point))
        {
            visited.Add(point);
            var newEntries = point.Adjacent(map).Except(visited);
            foreach(var newEntry in newEntries)
            {
                queue.Enqueue(newEntry);
            }
        }
        return visited;
    }

    static Map SimplifiedMap(Map map, HashSet<Point> keep)
        => (from y in Enumerable.Range(0, map.Length)
            let row = map[y]
            select new string((from x in Enumerable.Range(0, row.Length)
                              let c = row[x]
                              select keep.Contains(new Point(x, y)) 
                              ? c
                              : '.'
                             ).ToArray())).ToArray();


    public record Point(int X, int Y)
    {
        public bool IsInside(Map map)
            => Y >= 0 && Y < map.Length
            && X >= 0 && Y < map[Y].Length;

        public Point West
            => new(X - 1, Y);

        public Point East
            => new(X + 1, Y);

        public Point North
            => new(X, Y - 1);

        public Point South
            => new(X, Y + 1);

        public IEnumerable<Point> Neighbors
            => [North, South, East, West];

        public IEnumerable<Point> Adjacent(Map map)
            => map[Y][X] switch
            {
                '|' => [North, South],
                '-' => [East, West],
                'L' => [North, East],
                'J' => [North, West],
                '7' => [South, West],
                'F' => [South, East],
                '.' => [],
                'S' => from neighbor in Neighbors
                       where neighbor.IsInside(map)
                       where neighbor.Adjacent(map).Contains(this)
                       select neighbor,
                var c => throw new Exception($"Unknown map element {c}")
            };

        public bool RequiresJumping(Map map, HashSet<Point> loop)
            => loop.Contains(this) && Adjacent(map).Contains(North);
    }
}