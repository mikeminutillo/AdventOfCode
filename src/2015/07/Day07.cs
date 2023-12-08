using System.Collections.Immutable;

namespace AdventOfCode._2015._07;

using Circuit = ImmutableDictionary<string, ushort>;

public class Day07 : AdventOfCodeBase<Day07>
{
    public override object? Solution1(string input)
        => string.Join(Environment.NewLine,
            Sort(
                Parse(input).ToArray()
            )
            .Aggregate(Circuit.Empty, (circuit, instruction) => instruction.Apply(circuit))
            .OrderBy(x => x.Key)
            .Select(x => $"{x.Key}: {x.Value}")
        );

    public override object? Solution2(string input)
        => string.Join(Environment.NewLine,
            Sort(
                Parse(input).Where(x => x.Output != "b")
                            .Concat(Parse("16076 -> b"))
                            .ToArray()
            )
            .Aggregate(Circuit.Empty, (circuit, instruction) => instruction.Apply(circuit))
            .OrderBy(x => x.Key)
            .Select(x => $"{x.Key}: {x.Value}")
        );

    static IEnumerable<Instruction> Parse(string input)
        => from line in input.AsLines()
           select
            Parse(line, @"^(\w+) -> (\w+)", i => i[0]) ??
            Parse(line, @"^(\w+) AND (\w+) -> (\w+)", i => (ushort)(i[0] & i[1])) ??
            Parse(line, @"^(\w+) OR (\w+) -> (\w+)", i => (ushort)(i[0] | i[1])) ??
            Parse(line, @"^(\w+) LSHIFT (\d+) -> (\w+)", i => (ushort)(i[0] << i[1])) ??
            Parse(line, @"^(\w+) RSHIFT (\d+) -> (\w+)", i => (ushort)(i[0] >> i[1])) ??
            Parse(line, @"^NOT (\w+) -> (\w+)", i => (ushort)~i[0]) ??
            throw new Exception($"Unknown instruction {line}");

    static Instruction? Parse(string line, string pattern, Func<ushort[], ushort> function)
        => Regex.Match(line, pattern) is { Success: true } match
        ? Create(match.Groups.Cast<Group>().Skip(1).Select(m => m.Value).ToArray(), function)
        : null;

    static Instruction Create(string[] wires, Func<ushort[], ushort> function)
        => new(wires[0..^1], wires[^1], function);

    static IEnumerable<Instruction> Sort(Instruction[] instructions)
        => TopologicalSort([], ImmutableArray.Create(instructions));

    static ImmutableArray<Instruction> TopologicalSort(ImmutableArray<Instruction> sorted, ImmutableArray<Instruction> unsorted)
        => unsorted is []
        ? sorted
        : TopologicalSortRec(
            sorted.AddRange(unsorted.Where(x => x.InputsSatisfiedBy(sorted))),
            unsorted
        );

    static ImmutableArray<Instruction> TopologicalSortRec(ImmutableArray<Instruction> sorted, ImmutableArray<Instruction> unsorted)
        => TopologicalSort(
            sorted,
            unsorted.RemoveRange(sorted));

    record Instruction(string[] Inputs, string Output, Func<ushort[], ushort> Function)
    {
        public Circuit Apply(Circuit circuit)
            => circuit.SetItem(
                Output,
                Function(
                    Inputs.Select(x => ushort.TryParse(x, out var value) ? value : circuit[x]).ToArray()
                ));

        public bool InputsSatisfiedBy(ImmutableArray<Instruction> inputs)
            => Inputs.All(x => ushort.TryParse(x, out var _) || inputs.Any(y => y.Output == x));
    }
}