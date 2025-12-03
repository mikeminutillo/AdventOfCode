using System.Collections;
using System.Linq;
using System.Numerics;

namespace AdventOfCode;

static class Extensions
{
    extension<T>(IEnumerable<T> source)
    {
        public N Product<N>(Func<T, N> selector) where N : INumber<N>
            => source.Aggregate(N.MultiplicativeIdentity, (x, y) => x * selector(y));

        public IEnumerable<(int rank, T item)> Ranked()
            => source.Select((item, index) => (index + 1, item));
    }

    extension<T>(IEnumerable<T> source) where T : INumber<T>
    {
        public T Product() => source.Aggregate(T.MultiplicativeIdentity, (x, y) => x * y);
    }

    extension<T>(T source) where T : INumber<T>
    {
        public IEnumerable<T> UpTo(T end)
            => source.Unfold(n => n + T.One).TakeWhile(n => n <= end).Prepend(source);
    }

    extension(string source)
    {
        public string[] AsLines() => source.Trim().Split('\n');

        public IEnumerable<string> GetDigitSets()
            => from match in Regex.Matches(source, @"\d+") select match.Value;

        public IEnumerable<T> ExtractNumbers<T>() where T : INumber<T>
            => source.GetDigitSets().Select(n => T.Parse(n, null));
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
