using NUnit.Framework;
using System.Text.RegularExpressions;

namespace AdventOfCode._2023._05;

public class Day05 : AdventOfCodeBase<Day05>
{
    [TestCase(98, ExpectedResult = 50)]
    [TestCase(99, ExpectedResult = 51)]
    [TestCase(100, ExpectedResult = null)]
    public long? Test1(long index)
    {
        var range = new MapRange(50, 98, 2);
        return range.Get(index);
    }

    public override object? Solution1(string input)
        => Almanac.Parse(input).GetLocations().Min();

    record Almanac(long[] Seeds, Map[] Maps)
    {
        public IEnumerable<long> GetLocations()
            => Seeds.Select(GetLocation);

        public long GetLocation(long seed)
            => Get("humidity", "location",
            Get("temperature", "humidity",
            Get("light", "temperature",
            Get("water", "light", 
            Get("fertilizer", "water",
            Get("soil", "fertilizer",
            Get("seed", "soil", seed)))))));

        public long Get(string from, string to, long index)
            => Maps.Single(x => x.From == from && x.To == to)
                .Get(index);

        public static Almanac Parse(string input)
        {
            var sections = input.Split($"{Environment.NewLine}{Environment.NewLine}");
            var seeds = sections[0].ExtractLongNumbers().ToArray();
            var maps = sections.Skip(1).Select(Map.Parse).ToArray();
            return new Almanac(seeds, maps);
        }
    }


    record Map(string From, string To, MapRange[] Maps)
    {
        public long Get(long index)
            => Maps.Aggregate<MapRange, long?>(null, (x, y) => y.Get(index) ?? x)
            ?? index;

        public static Map Parse(string input)
        {
            var lines = input.Trim().AsLines();
            var titleMatch = Regex.Match(lines[0], @"([^-]+)-to-([^-]+) map:");
            var from = titleMatch.Result("$1");
            var to = titleMatch.Result("$2");
            return new Map(
                from,
                to,
                lines.Skip(1).Select(MapRange.Parse).ToArray()
            );
        }
    }

    record MapRange(long DestinationStart, long SourceStart, long Range)
    {
        public long? Get(long index)
            => index >= SourceStart && index < SourceStart + Range
            ? DestinationStart + (index - SourceStart)
            : null;

        public static MapRange Parse(string input)
        {
            var nums = input.ExtractLongNumbers().ToArray();
            return new MapRange(nums[0], nums[1], nums[2]);
        }
    }
}
