using System.Collections.Concurrent;
using System.Collections.Immutable;
namespace AdventOfCode._2023._12;

using Cache = ConcurrentDictionary<(string pattern, ImmutableQueue<int> toMatch), long>;

public class Day12 : AdventOfCodeBase<Day12>
{
    Cache cache = new Cache();
    ConcurrentDictionary<string, int> cache2 = new();

    public override object? Solution1(string input)
        => (from line in input.AsLines().Skip(2).Take(1)//.AsParallel()
            let report = SpringReport.Parse(line)
            //let validConfigurations = report.GetValidConfigurations(cache)
            let validConfigurations = CountWays(report.Springs, report.DamagedGroups)
            let _ = $"{validConfigurations,3}: {report}".Dump()
            select validConfigurations
           )
        .Sum();

    public override object? Solution2(string input)
        => (from line in input.AsLines()//.AsParallel()
            let report = SpringReport.Parse(line).Unfold(5)
            //let _ = report.Dump()
            let validConfigurations = report.GetValidConfigurations(cache)
            //let _ = $"{validConfigurations,3}: {report}".Dump()
            select validConfigurations
            //select 1
           ).Take(1).Sum();

    [TestCase("####", 1, ExpectedResult = 0)]
    [TestCase("?##?", 3, ExpectedResult = 2)]
    [TestCase("?##?", 2, ExpectedResult = 1)]
    [TestCase("?#??", 2, ExpectedResult = 2)]
    [TestCase("????", 4, ExpectedResult = 1)]
    [TestCase("????", 3, ExpectedResult = 2)]
    [TestCase("????", 2, ExpectedResult = 3)]
    [TestCase("????", 1, ExpectedResult = 4)]
    [TestCase("?##", 3, ExpectedResult = 1)]
    public int Foo(string input, int target)
        => CountWays(input.AsSpan(), target);


    // Break the string down into segments until it is the same length as the list of targets
    // Then compare each segment to the targets
    // And multiply the results

    int CountWays(string springs, ImmutableArray<int> damagedGroups)
        => CountWays(springs, Split(springs, '.').ToImmutableArray(), damagedGroups, []);

    IEnumerable <Range> Split(string s, char c)
    {
        var start = 0;
        for(var i = 0; i < s.Length; i++)
        {
            if (s[i] == c)
            {
                if(start != i)
                    yield return new Range(start, i);
                start = i + 1;
            }
        }
        if(start < s.Length)
        {
            yield return new Range(start, s.Length);
        }
    }

    int CountWays(string springs, ImmutableArray<Range> segments, ImmutableArray<int> damagedGroups, HashSet<ImmutableArray<Range>> visited)
    {
        // If segments is equal in size to damagedGroups
        // CountWays on each of them
        // Multiply them together
        // Otherwise, find all of the ways to break the groups up
        // And Recursively call CountWays on each
        //string.Join("|", segments.Select(x => springs[x])).Dump();
        if(visited.Contains(segments))
        {
            return 0;
        }

        visited.Add(segments);

        if (segments.Length == damagedGroups.Length)
        {
            var result = Enumerable.Zip(segments, damagedGroups)
                .Product(x => CountWays(springs[x.First], x.Second));
            if(result > 0)
                $"""
                MATCH
                {springs}
                {string.Join("|", segments.Select(x => springs[x]))}
                RESULT: {result}
                """.Dump();
            return result;
        }
        else if(segments.Length > damagedGroups.Length)
        {
            return 0;
        }

        return (from i in Enumerable.Range(0, segments.Length)
                let segment = segments[i]
                let segmentDetails = segment.GetOffsetAndLength(springs.Length)
                from j in Enumerable.Range(segmentDetails.Offset, segmentDetails.Length)
                where springs[j] == '?' // Replace with .
                && segmentDetails.Length > 1
                let beforeSegment = new Range(segmentDetails.Offset, j)
                let afterSegment = new Range(j + 1, segmentDetails.Offset + segmentDetails.Length)
                let toAdd = new[] { beforeSegment, afterSegment }
                            .Where(x => x.GetOffsetAndLength(springs.Length) is { Length: > 0 })
                            .ToArray()
                where toAdd.Length != 0
                select CountWays(
                    springs, 
                    segments.RemoveAt(i)
                    .InsertRange(i, toAdd), 
                    damagedGroups,
                    visited)
                ).Sum();
    }




    public int CountWays(ReadOnlySpan<char> input, int target)
        => (input.Length - target) < 0
            ? 0
            : CountWaysWithHashIndexes(input.Length, target, input.IndexOf('#'), input.LastIndexOf('#'));

    public int CountWaysWithHashIndexes(int length, int target, int indexOfFirst, int indexOfLast)
        => indexOfFirst == -1 && indexOfLast == -1
        ? length - target + 1 // Ways of getting contiguous sequences of # of length target into a string
        : (target - (indexOfLast - indexOfFirst + 1)) switch
            {
                <0 => 0, // There is not enough springs between the #s to reach the target
                 0 => 1, // There is exactly enough springs between the #s to reach the target
                var remaining => // There are remaining extra springs to get
                    Math.Min(indexOfFirst, remaining) // Springs before
                  + Math.Min(length - (indexOfLast + 1), remaining) // Springs after
            };

    public record SpringReport(string Springs, ImmutableArray<int> DamagedGroups)
    {
        public static SpringReport Parse(string s)
            => Create(s.Split(" "));

        static SpringReport Create(string[] strings)
            => new(
                strings[0],
                strings[1].ExtractNumbers().ToImmutableArray()
                );

        public SpringReport Unfold(int times)
            => new(
                string.Join("?", Enumerable.Repeat(Springs, times)),
                Enumerable.Repeat(DamagedGroups, times).Aggregate((a, b) => a.AddRange(b))
                );

        public override string ToString()
            => $"{Springs} {string.Join(",", DamagedGroups)}";

        public long GetValidConfigurations(Cache cache)
            => GetValidConfigurations(Springs, [.. DamagedGroups], cache);

        long GetValidConfigurations(string pattern, ImmutableQueue<int> toMatch, Cache cache)
            => cache.GetOrAdd(
                (pattern, toMatch),
                (key, c) => key.pattern switch
                    {
                        ['.', ..] => ProcessDot(key.pattern, key.toMatch, c),
                        ['?', ..] => ProcessQuestion(key.pattern, key.toMatch, c),
                        ['#', ..] => ProcessHash(key.pattern, key.toMatch, c),
                        _ => key.toMatch.IsEmpty ? 1 : 0
                    }, cache);

        long ProcessDot(string pattern, ImmutableQueue<int> toMatch, Cache cache)
            => GetValidConfigurations(pattern[1..], toMatch, cache);

        long ProcessQuestion(string pattern, ImmutableQueue<int> toMatch, Cache cache)
            => GetValidConfigurations("." + pattern[1..], toMatch, cache)
             + GetValidConfigurations("#" + pattern[1..], toMatch, cache);

        long ProcessHash(string pattern, ImmutableQueue<int> toMatch, Cache cache)
        {
            if(toMatch.IsEmpty) return 0;

            var maybeDead = pattern.IndexOf('.') switch
            {
                -1 => pattern.Length,
                var x => x
            };

            var n = toMatch.Peek();

            if (maybeDead < n) return 0;
            if (pattern.Length == n) return GetValidConfigurations("", toMatch.Dequeue(), cache);
            if (pattern[n] == '#') return 0;

            return GetValidConfigurations(pattern[(n + 1)..], toMatch.Dequeue(), cache);
        }
    }
}