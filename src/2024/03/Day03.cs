namespace AdventOfCode._2024._03;

public partial class Day03 : AdventOfCodeBase<Day03>
{
    public override object? Solution1(string input)
        => Solve(Solution1Regex(), input);

    public override object? Solution2(string input)
        => Solve(Solution2Regex(), input);

    [TestCase(
        "xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))",
        ExpectedResult = 161
    )]
    public int Sample1(string input)
        => Solve(Solution1Regex(), input);

    [TestCase(
        @"xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))",
        ExpectedResult = 48
    )]
    public int Sample2(string input)
        => Solve(Solution2Regex(), input);

    int Solve(Regex regex, string input)
        => regex.Matches(input)
                .Select(Parse)
                .Aggregate(new ProgramState(), (state, op) => op.Execute(state))
                .Accumulator;

    record ProgramState(bool MulEnabled = true, int Accumulator = 0);

    interface IOperation
    {
        ProgramState Execute(ProgramState input);
    }

    record MulOperation(int A, int B) : IOperation
    {
        public ProgramState Execute(ProgramState input)
            => input.MulEnabled
                ? input with
                {
                    Accumulator = input.Accumulator + (A * B)
                }
                : input;
    }

    record DoOperation() : IOperation
    {
        public ProgramState Execute(ProgramState input)
            => input with { MulEnabled = true };
    }

    record DontOperation() : IOperation
    {
        public ProgramState Execute(ProgramState input)
            => input with { MulEnabled = false };
    }

    IOperation Parse(Match match)
        => match.Groups["keyword"].Value switch
        {
            "do" => new DoOperation(),
            "don't" => new DontOperation(),
            "mul" => new MulOperation(
                int.Parse(match.Groups["a"].Value),
                int.Parse(match.Groups["b"].Value)
            ),
            _ => throw new Exception($"Dunno what a {match.Value} is")
        };

    [GeneratedRegex(@"((?<keyword>mul)\((?<a>\d{1,3}),(?<b>\d{1,3})\))|((?<keyword>do)\(\))|((?<keyword>don't)\(\))", RegexOptions.Multiline)]
    private static partial Regex Solution2Regex();

    [GeneratedRegex(@"((?<keyword>mul)\((?<a>\d{1,3}),(?<b>\d{1,3})\))", RegexOptions.Multiline)]
    private static partial Regex Solution1Regex();
}