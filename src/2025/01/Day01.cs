namespace AdventOfCode._2025._01;

public class Day01 : AdventOfCodeBase<Day01>
{
    public override object? Solution1(string input)
        => input.AsLines()
            .Aggregate<string, DialPosition[][], int>(
                [[new DialPosition()]],
                (state, instruction) => [..state, state[^1][^1].Follow(instruction)], 
                state => state.Count(x => x[^1].Position == 0)
            );

    public override object? Solution2(string input)
        => input.AsLines()
            .Aggregate<string, DialPosition[], int>(
                [new DialPosition()],
                (state, instruction) => [.. state, .. state[^1].Follow(instruction)],
                state => state.Count(x => x.Position == 0)
            );

    record DialPosition(int Position = 50)
    {
        public DialPosition TurnLeft()
            => new(Position == 0 ? 99 : Position - 1);

        public DialPosition TurnRight()
            => new(Position == 99 ? 0 : Position + 1);

        public DialPosition[] Follow(string instruction)
            => (instruction[0], int.Parse(instruction[1..])) switch
            {
                ('L', int steps) => [.. Apply([this], p => p.TurnLeft(), steps)],
                ('R', int steps) => [.. Apply([this], p => p.TurnRight(), steps)],
                _ => throw new Exception($"Unknown instruction: {instruction}")
            };
    }

    static IEnumerable<T> Apply<T>(T[] list, Func<T, T> next, int count)
        => count switch
        {
            < 0 => throw new Exception("WHUT?!"),
            0 => list[1..],
            _ => Apply([.. list, next(list[^1])], next, count - 1)
        };
}