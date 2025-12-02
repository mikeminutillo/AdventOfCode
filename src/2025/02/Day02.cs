namespace AdventOfCode._2025._02;

public class Day02 : AdventOfCodeBase<Day02>
{
    override public object? Solution1(string input)
        => Check(input, IsDoubled);

    public override object? Solution2(string input)
        => Check(input, IsMadeOfRepeats);

    static decimal Check(string input, Func<ReadOnlySpan<char>, bool> check)
        => (
            from range in input.Split(',')
            let numbers = range.ExtractNumbers<decimal>().ToArray()
            let start = numbers[0]
            let end = numbers[1]
            from number in start.Unfold(x => x + 1).TakeWhile(x => x <= end).Prepend(start)
            let id = number.ToString()
            where !check(id)
            let _ = $"{range}: {id}".Dump()
            select number
            ).Sum();

    static bool IsDoubled(ReadOnlySpan<char> id)
        => (id.Length / 2) switch
        {
            var midpoint => !(id[..midpoint].SequenceEqual(id[midpoint..]))
        };

    static bool IsMadeOfRepeats(ReadOnlySpan<char> id)
    {
        for(var i = id.Length / 2; i > 0; i--)
        {
            if(CheckForRepeats(id, i))
            {
                return false;
            }
        }
        return true;
    }

    static bool CheckForRepeats(ReadOnlySpan<char> id, int repeatLength)
    {
        ReadOnlySpan<char> current = default;
        var count = 0;

        for(var i = 0; i <= id.Length - repeatLength; i += repeatLength)
        {
            var next = id.Slice(i, repeatLength);
            if(count > 0)
            {
                if (!current.SequenceEqual(next))
                {
                    return false;
                }
            }
            count++;
            current = next;
        }

        return count * repeatLength == id.Length;
    }
}