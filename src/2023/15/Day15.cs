using System.Collections.Immutable;

namespace AdventOfCode._2023._15;

public class Day15 : AdventOfCodeBase<Day15>
{
    public override object? Solution1(string input)
        => (from step in input.Trim().Split(',')
            let hash = Hash(step)
            let _ = $"{step} => {hash}".Dump()
            select hash
            ).Sum();

    public override object? Solution2(string input)
        => FocusingPower(
            input.Trim().Split(",")
                .Select(Instruction.Parse)
                .Aggregate(
                    ImmutableDictionary.Create<int, Box>(),
                    (boxes, instruction) => instruction.Apply(boxes)
                )
            ).Dump();

    static int Hash(string input)
        => input.Aggregate(0, (hash, c) => ((hash + c) * 17) % 256);

    static long FocusingPower(ImmutableDictionary<int, Box> boxes)
        => (from box in boxes.Values
            from lensIndex in Enumerable.Range(0, box.Lenses.Length)
            let lens = box.Lenses[lensIndex]
            let focalPower = (box.Number + 1) * (lensIndex + 1) * lens.FocalLength
            let _ = $"{lens.Code}: {focalPower}".Dump()
            select focalPower)
        .Sum();

    record Instruction(string Lens, string Operation)
    {
        public static Instruction Parse(string instruction)
            => Regex.Match(instruction, @"^(\w+)(-|=\d+)") is { Success: true } match
            ? new(match.Result("$1"), match.Result("$2"))
            : throw new Exception($"Unknown instruction {instruction}");

        public ImmutableDictionary<int, Box> Apply(ImmutableDictionary<int, Box> state)
            => Hash(Lens) switch
            {
                var hash => state.TryGetValue(hash, out var box)
                    ? state.SetItem(hash, Apply(box))
                    : state.SetItem(hash, Apply(Box.Create(hash)))
            };

        Box Apply(Box box)
            => Operation[0] switch
                {
                    '-' => box.RemoveLens(Lens),
                    '=' => box.SetLens(Lens, int.Parse(Operation[1..])),
                    var x => throw new Exception($"Unknown instruction {x}")
                };
    }

    record Lens(string Code, int FocalLength);

    record Box(int Number, ImmutableArray<Lens> Lenses)
    {
        Lens? FindLens(string code)
            => Lenses.FirstOrDefault(l => l.Code == code);

        public Box RemoveLens(string lens)
            => new(Number, FindLens(lens) switch
            {
                null => Lenses,
                var existingLens => Lenses.Remove(existingLens)
            });

        public Box SetLens(string lens, int focalLength)
            => new Lens(lens, focalLength) switch
            {
                var newLens => new(Number, FindLens(lens) switch
                {
                    null => Lenses.Add(newLens),
                    var existingLens => Lenses.Replace(existingLens, newLens)
                })
            };

        public static Box Create(int number)
            => new(number, []);
    }
}