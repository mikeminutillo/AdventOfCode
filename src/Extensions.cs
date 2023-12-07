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

    public static IEnumerable<string> GetDigitSets(this string source)
        => from match in Regex.Matches(source, @"\d+") select match.Value;

    public static IEnumerable<int> ExtractNumbers(this string source)
        => source.GetDigitSets().Select(int.Parse);

    public static IEnumerable<long> ExtractLongNumbers(this string source)
        => source.GetDigitSets().Select(long.Parse);

    public static IEnumerable<(int rank, T item)> Ranked<T>(this IEnumerable<T> source)
        => source.Select((item, index) => (index + 1, item));
}
