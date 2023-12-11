namespace AdventOfCode._2023._11;

using System.Collections.Immutable;
using StarMap = char[][];

public class Day11 : AdventOfCodeBase<Day11>
{
    public override object? Solution1(string input)
        => DistanceBetweenStars(Load(input), 2);

    public override object? Solution2(string input)
        => DistanceBetweenStars(Load(input), 1000000);

    StarMap Load(string input)
        => (from line in input.AsLines()
            select line.ToArray()).ToArray();

    long DistanceBetweenStars(StarMap map, long expansion)
        => DistanceBetweenStars(
            map,
            FindEmptyRows(map),
            FindEmptyColumns(map),
            expansion
        );

    long DistanceBetweenStars(StarMap map, ImmutableArray<int> emptyRows, ImmutableArray<int> emptyColumns, long expansion)
        => DistanceBetweenStars(
            map,
            FindStars(map).Select(s => s.Shift(emptyRows, emptyColumns, expansion))
                          .ToImmutableArray()
        );

    long DistanceBetweenStars(StarMap map, ImmutableArray<Star> stars)
        => (from i in Enumerable.Range(0, stars.Length - 1)
            from j in Enumerable.Range(i + 1, stars.Length - i - 1)
            select stars[i].DistanceTo(stars[j])).Sum();

    ImmutableArray<Star> FindStars(StarMap map)
        => (from y in Enumerable.Range(0, map.Length)
           from x in Enumerable.Range(0, map[y].Length)
           where map[y][x] == '#'
           select new Star(x, y)).ToImmutableArray();

    ImmutableArray<int> FindEmptyColumns(StarMap map)
        => (from col in Enumerable.Range(0, map[0].Length)
            where map.All(line => line[col] == '.')
            select col).ToImmutableArray();

    ImmutableArray<int> FindEmptyRows(StarMap map)
        => (from row in Enumerable.Range(0, map.Length)
            where map[row].All(c => c == '.')
            select row).ToImmutableArray();

    record Star(long X, long Y)
    {
        public Star Shift(ImmutableArray<int> emptyRows, ImmutableArray<int> emptyColumns, long offset)
            => new(
                X + emptyColumns.Count(col => col < X) * (offset-1),
                Y + emptyRows.Count(row => row < Y) * (offset-1)
                );

        public long DistanceTo(Star other)
            => Math.Abs(other.X - X) + Math.Abs(other.Y - Y);
    }
}