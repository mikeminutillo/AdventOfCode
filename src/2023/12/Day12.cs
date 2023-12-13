using System.Collections.Concurrent;
using System.Collections.Immutable;
namespace AdventOfCode._2023._12;

using Cache = ConcurrentDictionary<(string pattern, ImmutableStack<int> toMatch), long>;

public class Day12 : AdventOfCodeBase<Day12>
{
    Cache cache = new Cache();

    public override object? Solution1(string input)
        => (from line in input.AsLines().AsParallel()
            let report = SpringReport.Parse(line)
            let validConfigurations = report.GetValidConfigurations(cache)
            //let _ = $"{validConfigurations,3}: {report}".Dump()
            select validConfigurations
           )
        .Sum();

    public override object? Solution2(string input)
        => (from line in input.AsLines().AsParallel()
            let report = SpringReport.Parse(line).Unfold(5)
            let validConfigurations = report.GetValidConfigurations(cache)
            select validConfigurations
           ).Sum();

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
            => GetValidConfigurations(Springs, [.. DamagedGroups.Reverse()], cache);

        long GetValidConfigurations(string pattern, ImmutableStack<int> toMatch, Cache cache)
            => cache.GetOrAdd(
                (pattern, toMatch),
                (key, c) => key.pattern switch
                    {
                        ['.', ..] => ProcessDot(key.pattern, key.toMatch, c),
                        ['?', ..] => ProcessQuestion(key.pattern, key.toMatch, c),
                        ['#', ..] => ProcessHash(key.pattern, key.toMatch, c),
                        _ => key.toMatch.IsEmpty ? 1 : 0
                    }, cache);

        long ProcessDot(string pattern, ImmutableStack<int> toMatch, Cache cache)
            => GetValidConfigurations(pattern[1..], toMatch, cache);

        long ProcessQuestion(string pattern, ImmutableStack<int> toMatch, Cache cache)
            => GetValidConfigurations("." + pattern[1..], toMatch, cache)
             + GetValidConfigurations("#" + pattern[1..], toMatch, cache);

        long ProcessHash(string pattern, ImmutableStack<int> toMatch, Cache cache)
        {
            if(toMatch.IsEmpty) return 0;

            var maybeDead = pattern.IndexOf('.') switch
            {
                -1 => pattern.Length,
                var x => x
            };

            var n = toMatch.Peek();

            if (maybeDead < n) return 0;
            if (pattern.Length == n) return GetValidConfigurations("", toMatch.Pop(), cache);
            if (pattern[n] == '#') return 0;

            return GetValidConfigurations(pattern[(n + 1)..], toMatch.Pop(), cache);
        }
    }
}