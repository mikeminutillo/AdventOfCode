using System.Collections;

namespace AdventOfCode;

static class Extensions
{
    public static T Dump<T>(this T obj)
    {
        TestContext.Out.WriteLine(obj switch
        {
            // HINT: String is Enumerable
            string o => o?.ToString(),
            IEnumerable enumerable => string.Join(", ", enumerable.Cast<object>()),
            var o => o?.ToString()
        });
        return obj;
    }

    public static string[] AsLines(this string input)
        => input.Trim().Split("\n");

    public static int Product<T>(this IEnumerable<T> source, Func<T, int> selector)
        => source.Aggregate(1, (x, y) =>  x * selector(y));

    public static int Product(this IEnumerable<int> source)
        => source.Aggregate(1, (x, y) => x * y);

    public static IEnumerable<int> ExtractNumbers(this string source)
        => from match in Regex.Matches(source, @"\d+") select int.Parse(match.Value);

    public static IEnumerable<long> ExtractLongNumbers(this string source)
        => from match in Regex.Matches(source, @"\d+") select long.Parse(match.Value);
}
