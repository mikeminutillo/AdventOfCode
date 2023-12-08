
using System.Collections.Immutable;

namespace AdventOfCode._2023._08;

public class Day08 : AdventOfCodeBase<Day08>
{
    public override object? Solution1(string input)
        => Map.Parse(input).DistanceTravelled("AAA", x => x == "ZZZ");

    public override object? Solution2(string input)
        => Map.Parse(input)
              .GhostDistanceTravelled(
                  x => x.EndsWith("A"),
                  x => x.EndsWith("Z")
              );

    record Map(string Directions, ImmutableDictionary<string, string> Left, ImmutableDictionary<string, string> Right)
    {
        IEnumerable<string> Locations
            => Left.Keys;

        public long DistanceTravelled(string start, Func<string, bool> destinationCheck)
        {
            if(Locations.Contains(start) == false)
            {
                $"Cannot find {start}".Dump();
                return -1;
            }
            var current = start;
            var distanceTraveled = 0;
            while (destinationCheck(current) == false)
            {
                current = Directions[distanceTraveled % Directions.Length] switch
                {
                    'L' => Left[current],
                    'R' => Right[current],
                    _ => current
                };
                distanceTraveled++;
            }
            $"{start} -> {current} ({distanceTraveled})".Dump();
            return distanceTraveled;
        }

        public long GhostDistanceTravelled(Func<string, bool> startCheck, Func<string, bool> endCheck)
            => LeastCommonMultiple(
                     from loc in Locations
                     where startCheck(loc)
                     select DistanceTravelled(loc, endCheck)
                );

        long LeastCommonMultiple(IEnumerable<long> nums)
            => nums.Aggregate(LeastCommonMultiple);

        long LeastCommonMultiple(long a, long b)
            => a * b / GreatestCommonDivisor(a, b);

        long GreatestCommonDivisor(long a, long b)
            => b == 0 ? a : GreatestCommonDivisor(b, a % b);

        public static Map Parse(string input)
            => Create(input.AsLines());

        static Map Create(string[] lines)
            => new(lines[0],
                lines.Skip(2).ToImmutableDictionary(x => x[..3], x => x[7..10]),
                lines.Skip(2).ToImmutableDictionary(x => x[..3], x => x[12..15])
               );
    }
}