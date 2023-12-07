namespace AdventOfCode._2015._02;

public class Day02 : AdventOfCodeBase<Day02>
{
    public override object? Solution1(string input)
        => input.AsLines()
                .Select(Box.Parse)
                .Sum(b => b.PaperNeeded);

    public override object? Solution2(string input)
        => input.AsLines()
                .Select(Box.Parse)
                .Dump()
                .Sum(b => b.RibbonNeeded);

    record Box(int[] Dimensions)
    {
        IEnumerable<int> SideAreas
            => Enumerable.Zip(
                Dimensions,
                Dimensions.Skip(1).Concat(Dimensions.Take(1))
                ).Select(x => x.First * x.Second);

        public int RibbonNeeded
            => 2 * Dimensions.OrderBy(x => x).Take(2).Sum()
            + Dimensions.Product();

        public int PaperNeeded
            => SideAreas.Sum() * 2
            + SideAreas.Min();

        public static Box Parse(string input)
            => new(input.ExtractNumbers().ToArray());
    }
}