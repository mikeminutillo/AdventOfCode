namespace AdventOfCode._2015._06;

public class Day06 : AdventOfCodeBase<Day06>
{
    public override object? Solution1(string input) 
        => Solve(Load<bool>(input,
            turnOn: _ => true,
            turnOff: _ => false,
            toggle: c => !c
          ))
        .Count(x => x);

    public override object? Solution2(string input)
        => Solve(Load<int>(input,
            turnOn: c => c + 1,
            turnOff: c => Math.Max(0, c - 1),
            toggle: c => c + 2
        ))
        .Sum();

    static IEnumerable<T> Solve<T>(Instruction<T>[] instructions) where T : struct
        => from x in Enumerable.Range(0, 1000).AsParallel()
           from y in Enumerable.Range(0, 1000)
           let p = new Point(x, y)
           select instructions.Aggregate(
               default(T),
               (current, instruction) => instruction.Adjust(p, current)
           );

    static Instruction<T>[] Load<T>(string input, Func<T, T> turnOn, Func<T, T> turnOff, Func<T, T> toggle)
    => (from line in input.AsLines()
       let instruction = Regex.Match(line, @"[^\d]+").Value.Trim()
       let nums = line.ExtractNumbers().ToArray()
       let start = new Point(nums[0], nums[1])
       let end = new Point(nums[2], nums[3])
       select new Instruction<T>(start, end, instruction switch
       {
           "turn off" => turnOff,
           "turn on" => turnOn,
           "toggle" => toggle,
           _ => throw new Exception($"Unknown instruction {instruction}")
       })).ToArray();

    record Point(int X, int Y);

    record Instruction<T>(Point Start, Point End, Func<T, T> Action)
    {
        bool Contains(Point p)
            => p.X >= Start.X && p.X <= End.X
            && p.Y >= Start.Y && p.Y <= End.Y;

        public T Adjust(Point location, T currentValue)
            => Contains(location)
            ? Action(currentValue)
            : currentValue;
    }
}