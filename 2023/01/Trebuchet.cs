using NUnit.Framework;
using System.Text.RegularExpressions;

namespace AdventOfCode._2023._01;

public class Trebuchet : AdventOfCodeBase
{
    [TestCase]
    public void Example()
    {
        var input = Load("sample");
    
        var result = Sum(input);

        Approve(result);
    }

    [TestCase]
    public void Example2()
    {
        var input = Load("sample2");

        var result = SumWithWords(input);

        Approve(result);
    }

    [TestCase]
    public void Problem1()
    {
        var input = Load("input");

        var result = Sum(input);

        Approve(result);
    }

    [TestCase]
    public void Problem2() 
    {
        var input = Load("input");

        var result = SumWithWords(input);

        Approve(result);
    }

    int Sum(string input) => (from line in input.Split(Environment.NewLine)
                              let first = line.First(char.IsDigit)
                              let last = line.Last(char.IsDigit)
                              select int.Parse($"{first}{last}")).Sum();

    int SumWithWords(string input) => (from line in input.Split(Environment.NewLine)
                                        let digits = ToDigits(line).ToArray().Dump()
                                        let first = digits.First()
                                        let last = digits.Last()
                                        select (first * 10) + last
                                        ).Sum();

    IEnumerable<int> ToDigits(string line)
    {
        foreach(var digit in GetDigits(line)) 
        {
            if(digit.Length == 1)
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
        while(match.Success)
        {
            yield return match.Value;
            match = regex.Match(input, match.Index + 1);
        }
    }
}
