using System.Collections.Immutable;
using System.Text;

namespace AdventOfCode._2015._10;

public partial class Day10 : AdventOfCodeBase<Day10>
{
    public override object? Solution1(string input)
        => LookAndSay(input, 40).Length;

    public override object? Solution2(string input)
        => LookAndSay(input, 50).Length;

    static string LookAndSay(string input, int iterations)
        => Enumerable.Range(0, iterations)
                     .Aggregate(
                         input.Trim(),
                         (i, _) => LookAndSay(i)
                      );

    static string LookAndSay(string input)
        => input.Aggregate(
                ImmutableStack.Create<(char character, int count)>(),
                (stack, c) => !stack.IsEmpty && stack.Peek().character == c
                    ? stack.Pop(out var rec).Push((c, rec.count + 1))
                    : stack.Push((c, 1))
            )
            .Reverse()
            .Aggregate(
                new StringBuilder(),
                (sb, seq) => sb.Append(seq.count).Append(seq.character)
            ).ToString();
}