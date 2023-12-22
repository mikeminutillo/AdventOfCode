using System.Diagnostics;

namespace AdventOfCode.Infrastructure;

static class Track
{
    class Timer(string name) : IDisposable
    {
        Stopwatch stopWatch = Stopwatch.StartNew();

        void IDisposable.Dispose()
        {
            $"{name}: {stopWatch.ElapsedMilliseconds}ms".Dump();
        }
    }

    public static IDisposable Time(string name)
        => new Timer(name);
}
