﻿using System.Text.RegularExpressions;

namespace AdventOfCode._2023._01;

public class Trebuchet : AdventOfCodeBase<Trebuchet>
{
    public override object Solution1(string input) => Sum(input);

    public override object Solution2(string input) => SumWithWords(input);

    int Sum(string input) => (from line in input.AsLines()
                              let first = line.FirstOrDefault(char.IsDigit)
                              let last = line.LastOrDefault(char.IsDigit)
                              where first != default && last != default
                              select int.Parse($"{first}{last}")).Sum();

    int SumWithWords(string input) => (from line in input.AsLines()
                                       let digits = ToDigits(line).ToArray().Dump()
                                       let first = digits.First()
                                       let last = digits.Last()
                                       select (first * 10) + last
                                        ).Sum();

    IEnumerable<int> ToDigits(string line)
    {
        foreach (var digit in GetDigits(line))
        {
            if (digit.Length == 1)
            {
                yield return int.Parse(digit);
            }
            else
            {
                yield return digit switch
                {
                    "one" => 1,
                    "two" => 2,
                    "three" => 3,
                    "four" => 4,
                    "five" => 5,
                    "six" => 6,
                    "seven" => 7,
                    "eight" => 8,
                    "nine" => 9,
                    _ => throw new Exception("Not a digit")
                };
            }
        }
    }

    IEnumerable<string> GetDigits(string input)
    {
        var regex = new Regex("(one|two|three|four|five|six|seven|eight|nine|[0-9])");
        var match = regex.Match(input);
        while (match.Success)
        {
            yield return match.Value;
            match = regex.Match(input, match.Index + 1);
        }
    }
}
