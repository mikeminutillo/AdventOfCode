using System.Collections;

namespace AdventOfCode;

static class Extensions
{
    extension<T>(IEnumerable<T> source)
    {
        public int Product(Func<T, int> selector)
            => source.Aggregate(1, (x, y) => x * selector(y));

        public IEnumerable<(int rank, T item)> Ranked()
            => source.Select((item, index) => (index + 1, item));
    }

    extension(IEnumerable<int> source)
    {
        public int Product() => source.Aggregate(1, (x, y) => x * y);
    }

    extension(string source)
    {
        public string[] AsLines() => source.Trim().Split('\n');

        public IEnumerable<string> GetDigitSets()
            => from match in Regex.Matches(source, @"\d+") select match.Value;

        public IEnumerable<int> ExtractNumbers()
            => source.GetDigitSets().Select(int.Parse);

        public IEnumerable<long> ExtractLongNumbers()
            => source.GetDigitSets().Select(long.Parse);
    }

    extension<T>(T seed)
    {
        public IEnumerable<T> Unfold(Func<T, T> next)
        {
            var current = next(seed);
            while(true)
            {
                yield return current;
                current = next(current);
            }
        }

        public T Dump()
        {
            TestContext.Out.WriteLine(seed switch
            {
                // HINT: String is Enumerable
                string o => o?.ToString(),
                IEnumerable enumerable => string.Join(", ", enumerable.Cast<object>()),
                var o => o?.ToString()
            });
            return seed;
        }
    }
}
