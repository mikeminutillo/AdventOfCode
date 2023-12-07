using System.Security.Cryptography;
using System.Text;

namespace AdventOfCode._2015._04;

public class Day04 : AdventOfCodeBase<Day04>
{
    readonly MD5 MD5 = MD5.Create();

    public override object? Solution1(string input)
        => FindHashes(input.Trim(), "00000").First();

    public override object? Solution2(string input)
        => FindHashes(input.Trim(), "000000").First();

    IEnumerable<int> FindHashes(string input, string prefix)
        => from i in Enumerable.Range(0, int.MaxValue)
           let bytes = Encoding.ASCII.GetBytes($"{input}{i}")
           let hash = Convert.ToHexString(MD5.ComputeHash(bytes))
           where hash.StartsWith(prefix)
           select i;
}