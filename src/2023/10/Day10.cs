namespace AdventOfCode._2023._10;

using System.Collections.Immutable;
using Map = string[];

public class Day10 : AdventOfCodeBase<Day10>
{
    public override object? Solution1(string input)
        => FindLoop(input.AsLines()).Count / 2;

    static Point FindStart(Map map)
        => (from row in Enumerable.Range(0, map.Length)
            let sPosition = map[row].IndexOf('S')
            where sPosition > -1
            select new Point(sPosition, row)).Single();

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

    record Point(int X, int Y)
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
    }
}