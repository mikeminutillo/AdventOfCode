namespace AdventOfCode._2025._01;

public class Day01 : AdventOfCodeBase<Day01>
{
    public override object? Solution1(string input)
        => input.AsLines()
            .Aggregate(
                (position: new DialPosition(50), count: 0),
                (state, instruction) => state.position.Follow(instruction).Last() switch
                {
                    var finalPosition => (
                        finalPosition,
                        finalPosition.Position == 0
                            ? state.count + 1
                            : state.count
                    )
                },
                state => state.count
            );

    public override object? Solution2(string input)
        => input.AsLines()
            .Aggregate(
                (position: new DialPosition(50), count: 0),
                (state, instruction) => state.position.Follow(instruction).ToArray() switch
                {
                    var positions => (
                        positions.Last(),
                        state.count + positions.Count(p => p.Position == 0)
                    )
                },
                state => state.count
            );

    record DialPosition(int Position)
    {
        public DialPosition TurnLeft()
            => new((Position - 1) % 100);

        public DialPosition TurnRight()
            => new((Position + 1) % 100);

        public IEnumerable<DialPosition> Follow(string instruction)
            => (instruction[0], int.Parse(instruction[1..])) switch
            {
                ('L', int steps) => this.Unfold(p => p.TurnLeft()).Take(steps),
                ('R', int steps) => this.Unfold(p => p.TurnRight()).Take(steps),
                _ => throw new Exception($"Unknown instruction: {instruction}")
            };
    }
}