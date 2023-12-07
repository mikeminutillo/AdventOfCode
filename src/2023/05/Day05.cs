namespace AdventOfCode._2023._05;

public class Day05 : AdventOfCodeBase<Day05>
{
    public override object? Solution1(string input)
        => Almanac.Parse(input).LowestLocationNumber;

    public override object? Solution2(string input)
        => Almanac.Parse(input, treatSeedsLineAsRanges: true).LowestLocationNumber;

    record Almanac(Range[] Seeds, AlmanacMap[] Maps)
    {
        public long LowestLocationNumber =>
            Maps.Aggregate(
                Seeds.AsEnumerable(),
                (inputs, map) => map.ApplyTo(inputs)
            ).Min(x => x.Start);

        public static Almanac Parse(string input, bool treatSeedsLineAsRanges = false)
        {
            var sections = input.Split($"\n\n");
            var seeds = treatSeedsLineAsRanges 
                ? sections[0]
                    .ExtractLongNumbers()
                    .Chunk(2)
                    .Select(x => new Range(x[0], x[0] + x[1] - 1))
                    .ToArray()
                : sections[0].ExtractLongNumbers()
                    .Select(x => new Range(x, x))
                    .ToArray();
            var maps = sections.Skip(1).Select(AlmanacMap.Parse).ToArray();
            return new Almanac(seeds, maps);
        }
    }

    record AlmanacMap(string From, string To, AlmanacMapping[] Mappings)
    {
        public IEnumerable<Range> ApplyTo(IEnumerable<Range> inputs)
        {
            var queue = new Queue<Range>(inputs);

            while(queue.TryDequeue(out var input))
            {
                var handled = false;
                foreach(var mapping in Mappings)
                {
                    var intersection = mapping.Source.Intersection(input);
                    if(intersection != null)
                    {
                        var shiftStart = mapping.Destination.Start + intersection.Start - mapping.Source.Start;
                        handled = true;
                        var shifted = intersection.MovedTo(shiftStart);
                        yield return shifted;

                        $"{From}->{To}: Shifted {intersection} to {shifted}".Dump();

                        foreach (var remainder in input.Disjunction(intersection))
                        {
                            $"{From}->{To}: REMAINDER {remainder} added for processing".Dump();
                            queue.Enqueue(remainder);
                        }
                        break;
                    }
                }
                if(!handled)
                {
                    $"{From}->{To}: {input} not handled. Falling through".Dump();
                    yield return input;
                }
            }
        }

        public static AlmanacMap Parse(string input)
        {
            var lines = input.Trim().AsLines();
            var titleMatch = Regex.Match(lines[0], @"([^-]+)-to-([^-]+) map:");
            var from = titleMatch.Result("$1");
            var to = titleMatch.Result("$2");
            return new AlmanacMap(
                from,
                to,
                lines.Skip(1).Select(AlmanacMapping.Parse).ToArray()
            );
        }
    }

    record AlmanacMapping(Range Source, Range Destination)
    {
        public static AlmanacMapping Parse(string input)
        {
            var nums = input.ExtractLongNumbers().ToArray();
            var destinationStart = nums[0];
            var sourceStart = nums[1];
            var length = nums[2];

            return new AlmanacMapping(
                new Range(sourceStart, sourceStart + length + 1),
                new Range(destinationStart, destinationStart + length + 1)
            );
        }
    }

    record Range(long Start, long End)
    {
        public long Length => End - Start + 1;

        public Range MovedTo(long newStart)
            => new Range(newStart, newStart + Length - 1);

        public Range? Intersection(Range other)
            => other.Start > End || Start > other.End
            ? null
            : new Range(
                Math.Max(Start, other.Start),
                Math.Min(End, other.End)
            );

        public IEnumerable<Range> Disjunction(Range other)
        {
            if (Start < other.Start && End >= other.Start)
                yield return new Range(Start, other.Start - 1);
            if (End > other.End && Start < other.End)
                yield return new Range(other.End + 1, End);
        }

        public override string ToString()
            => $"[{Start}..{End}]";
    }
}
