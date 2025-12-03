namespace AdventOfCode._2025._03;

public class Day03 : AdventOfCodeBase<Day03>
{
    public override object? Solution1(string input) => Solve(input, 2);

    public override object? Solution2(string input) => Solve(input, 12);

    static decimal Solve(string input, int batteryCount)
        => input.AsLines()
            .Select(b => MaxJoltage(b, batteryCount).Dump())
            .Sum();

    static decimal MaxJoltage(string joltages, int digitCount)
        => GetHighestSequenceOfDigits(joltages, digitCount)
            .Aggregate(0m, (acc, d) => acc * 10 + (d - '0'));

    static IEnumerable<char> GetHighestSequenceOfDigits(ReadOnlySpan<char> digits, int count)
        => count switch
        {
            0 => [],
            _ => MaxWithIndex(digits[..^(count - 1)]) switch
            {
                (var digit, var index) => [
                    digit,
                        .. GetHighestSequenceOfDigits(digits[(index + 1)..], count -1)
                ]
            }
        };

    static (char digit, int index) MaxWithIndex(ReadOnlySpan<char> digits)
    {
        char max = default;
        var maxIndex = -1;
        for (var i = 0; i < digits.Length; i++)
        {
            if (digits[i] > max)
            {
                max = digits[i];
                maxIndex = i;
            }
        }
        return (max, maxIndex);
    }
}