using System.Collections.Immutable;

namespace AdventOfCode._2025._08;

public class Day08 : AdventOfCodeBase<Day08>
{
    public override object? Solution1(string input)
        => (input.AsLines()
            .Select(JunctionBox.Parse)
            .ToImmutableArray() switch
        {
            var boxes => (from i in Enumerable.Range(0, boxes.Length - 1)
                          from j in Enumerable.Range(i + 1, boxes.Length - i - 1)
                          let box = boxes[i]
                          let other = boxes[j]
                          let distance = box.DistanceTo(other)
                          orderby distance
                          select new { box, other })
            .Take(TestContext.CurrentContext.Test.Name.EndsWith("sample") ? 10 : 1000)
            .Aggregate(
                boxes.Select(b => new Circuit([b])).ToImmutableArray(),
                (circuits, pair) =>
                    (
                        circuits.Single(c => c.Contains(pair.box)),
                        circuits.Single(c => c.Contains(pair.other))
                    ) switch
                    {
                        (var first, var second) =>
                            ReferenceEquals(first, second)
                               ? circuits
                               : circuits.Remove(first)
                                    .Remove(second)
                                    .Add(first.Add(second))
                     
                    }, 
                circuits => circuits
            )
        })
        .OrderByDescending(c => c.Boxes.Count)
        .Take(3)
        .Product(c => c.Boxes.Count.Dump())
        .Dump();

    public override object? Solution2(string input)
        => (input.AsLines()
            .Select(JunctionBox.Parse)
            .ToImmutableArray() switch
        {
            var boxes => (from i in Enumerable.Range(0, boxes.Length - 1)
                          from j in Enumerable.Range(i + 1, boxes.Length - i - 1)
                          let box = boxes[i]
                          let other = boxes[j]
                          let distance = box.DistanceTo(other)
                          orderby distance
                          select new { box, other })
            .Aggregate(
                (
                    Circuits: boxes.Select(b => new Circuit([b])).ToImmutableArray(),
                    Distance: 0d
                ),
                (state, pair) =>
                    (
                        state.Circuits.Single(c => c.Contains(pair.box)),
                        state.Circuits.Single(c => c.Contains(pair.other))
                    ) switch
                    {
                        (var first, var second) =>
                            ReferenceEquals(first, second)
                               ? state
                               : state with
                               {
                                   Circuits = state.Circuits.Remove(first)
                                    .Remove(second)
                                    .Add(first.Add(second)),
                                   Distance = pair.box.X * pair.other.X
                               }
                    },
                state => state.Distance
            )
        }).Dump();

    record Circuit(ImmutableHashSet<JunctionBox> Boxes)
    {
        public Circuit Add(Circuit other)
            => new(Boxes.Union(other.Boxes));

        public bool Contains(JunctionBox box)
            => Boxes.Contains(box);
    }

    record JunctionBox(double X, double Y, double Z)
    {
        public double DistanceTo(JunctionBox other)
            => Math.Sqrt(Math.Pow(other.X - X, 2) + Math.Pow(other.Y - Y, 2) + Math.Pow(other.Z - Z, 2));


        public static JunctionBox Parse(string line)
            => line.ExtractNumbers<double>().ToArray() is { Length: 3 } parts
                ? new JunctionBox(parts[0], parts[1], parts[2])
                : throw new ArgumentException("Invalid input", nameof(line));
    }
}