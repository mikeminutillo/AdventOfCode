namespace AdventOfCode._2025._03;

public class Day03 : AdventOfCodeBase<Day03>
{
    public override object? Solution1(string input) => Solve(input, 2);

    public override object? Solution2(string input) => Solve(input, 12);

    static decimal Solve(string input, int batteryCount)
        => input.AsLines()
            .Select(BatteryBank.Parse)
            .Select(b => b.MaxJoltage(batteryCount).Dump())
            .Sum();

    record BatteryBank(int[] BatteryJoltages)
    {
        public decimal MaxJoltage(int digitCount)
            => GetHighestSequenceOfDigits(BatteryJoltages, digitCount)
                .Aggregate(0m, (acc, d) => acc * 10 + d);

        static IEnumerable<int> GetHighestSequenceOfDigits(ReadOnlySpan<int> digits, int count)
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

        static (int digit, int index) MaxWithIndex(ReadOnlySpan<int> digits)
        {
            var max = -1;
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

        public static BatteryBank Parse(string line)
            => new([.. line.ToArray().Select(c => c - '0')]);
    }
}