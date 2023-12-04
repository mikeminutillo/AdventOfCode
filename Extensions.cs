using NUnit.Framework;
using System.Collections;
using System.Text.RegularExpressions;

namespace AdventOfCode;

static class Extensions
{
    public static T Dump<T>(this T obj)
    {
        var output = obj?.ToString();

        if(obj is string)
        {
            // Do nothing
        }
        else if(obj is IEnumerable enumerable)
        {
            output = "";
            foreach(var item in enumerable)
            {
                output += $"{item}, ";
            }
        }

        TestContext.Out.WriteLine(output);
        return obj;
    }

    public static string[] AsLines(this string input)
        => input.Split(Environment.NewLine);

    public static int Product<T>(this IEnumerable<T> source, Func<T, int> selector)
        => source.Aggregate(1, (x, y) =>  x * selector(y));

    public static int Product(this IEnumerable<int> source)
        => source.Aggregate(1, (x, y) => x * y);

    public static IEnumerable<int> ExtractNumbers(this string source)
        => from match in Regex.Matches(source, @"\d+") select int.Parse(match.Value);
}
