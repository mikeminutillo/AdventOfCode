
using System.Collections.Immutable;
using System.Linq;

namespace AdventOfCode._2023._14;

public class Day14 : AdventOfCodeBase<Day14>
{
    public override object? Solution1(string input)
        => new Platform(input.AsLines())
                    .TiltNorth()
                    .Dump()
                    .Load();

    public override object? Solution2(string input)
        => CycleWithDetectedRepeats(
                new Platform(input.AsLines()), 
                1000000000
            ).Dump()
             .Load();

    Platform CycleWithDetectedRepeats(Platform platform, long iterations)
    {
        Dictionary<string, long> cache = new();
        var current = platform;
        for(long i = 1; i <= iterations; i++)
        {
            current = current.Cycle();
            if(cache.TryGetValue(current.ToString(), out var lastIteration))
            {
                var repeatLength = i - lastIteration;
                var remainingIterations = iterations - lastIteration;
                var loops = remainingIterations / repeatLength;
                var todo = iterations - (loops * repeatLength + lastIteration);
                return Enumerable.Range(0, (int)todo)
                    .Aggregate(current, (platform, _) => platform.Cycle());

            }
            cache[current.ToString()] = i;
        }
        return current;
    }

    record Platform(string[] Lines)
    {
        (Platform newState, bool changed) TiltNorthOneStep()
        {
            var newLines = Lines.Select(x => x.ToImmutableArray())
                .ToList();
            var changed = false;

            for (var i = 0; i < newLines.Count - 1; i++)
            {
                var currentLine = newLines[i].ToBuilder();
                var nextLine = newLines[i + 1].ToBuilder();

                for (var j = 0; j < currentLine.Count; j++)
                {
                    if (currentLine[j] == '.' && nextLine[j] == 'O')
                    {
                        currentLine[j] = 'O';
                        nextLine[j] = '.';
                        changed = true;
                    }
                }
                newLines[i] = currentLine.ToImmutable();
                newLines[i + 1] = nextLine.ToImmutable();
            }

            return (new Platform(
                newLines.Select(
                    x => new string(x.ToArray())
                ).ToArray()
            ), changed);
        }

        public Platform TiltNorth()
        {
            var current = this;
            while (true)
            {
                var (newState, changed) = current.TiltNorthOneStep();
                if (!changed)
                    return newState;
                current = newState;
            }
        }

        public Platform Cycle()
            => Enumerable.Range(0, 4)
                .Aggregate(
                    this, 
                    (platform, _) => platform.TiltNorth()
                                             .RotateClockwise()
                );

        public Platform RotateClockwise()
            => new(
                (from x in Enumerable.Range(0, Lines[0].Length)
                 select new string(Enumerable.Range(0, Lines.Length)
                                            .Reverse()
                                            .Select(y => Lines[y][x])
                                            .ToArray())
                 ).ToArray());

        public int Load() =>
            (from i in Enumerable.Range(0, Lines.Length)
             let weight = Lines.Length - i
             let line  = Lines[i]
             let count = line.Count(x => x == 'O')
             let load = count * weight
             let _ = $"Line {i,2}: {count} x {weight} = {load}".Dump()
             select load
             
             ).Sum();

        public override string ToString()
            => string.Join(Environment.NewLine, Lines);
    }
}