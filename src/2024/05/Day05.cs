using System.Collections.Immutable;
using System.Data;

namespace AdventOfCode._2024._05;

public class Day05 : AdventOfCodeBase<Day05>
{
    public override object? Solution1(string input)
        => PrinterState.Parse(input)
                       .FindAllCorrectOrder()
                       .Select(x => x.Dump())
                       .Sum(x => x.MiddlePageNumber)
                       .Dump();

    public override object? Solution2(string input)
        => PrinterState.Parse(input)
                       .FindReorders()
                       .Select(x => x.Dump())
                       .Sum(x => x.MiddlePageNumber)
                       .Dump();

    record PrinterState(ImmutableArray<PageOrderRule> Rules, ImmutableArray<PageUpdate> Updates)
    {
        public static PrinterState Parse(string input)
            => input.Split("\n\n") switch
            {
                [var rules, var updates] => new(
                    [.. rules.AsLines().Select(PageOrderRule.Parse)],
                    [.. updates.AsLines().Select(PageUpdate.Parse)]
                ),
                _ => throw new Exception()
            };
        public IEnumerable<PageUpdate> FindAllCorrectOrder()
            => from update in Updates
               where Rules.All(update.Satisfies)
               select update;

        public IEnumerable<PageUpdate> FindReorders()
            => from update in Updates
               where Rules.All(update.Satisfies) == false
               select update.Reorder(Rules);
    }

    record PageOrderRule(int Left, int Right)
    {
        public static PageOrderRule Parse(string input)
            => input.ExtractNumbers<int>().ToArray() switch
            {
                [var left, var right] => new(left, right),
                _ => throw new Exception()
            };

        public int Order(int x, int y)
            => x == Left && y == Right
            ? -1
            : x == Right && y == Left
            ? 1
            : 0;
    }

    record PageUpdate(ImmutableArray<int> PagesToUpdate)
    {
        public static PageUpdate Parse(string input)
            => new(input.ExtractNumbers<int>().ToImmutableArray());

        public int MiddlePageNumber
            => PagesToUpdate[PagesToUpdate.Length / 2];

        public bool Satisfies(PageOrderRule rule)
            => (
                from page in PagesToUpdate
                let left = page == rule.Left
                let right = page == rule.Right
                where left || right
                select (left, right)
               )
            .SkipWhile(x => x.left)
            .SkipWhile(x => x.right)
            .Any() == false;

        public PageUpdate Reorder(ImmutableArray<PageOrderRule> rules)
            => new(
                PagesToUpdate.Sort(new RulesComparer(rules))
            );

        public override string ToString()
            => string.Join(", ", PagesToUpdate);
    }

    record RulesComparer(ImmutableArray<PageOrderRule> Rules) : IComparer<int>
    {
        public int Compare(int x, int y)
            => Rules.Sum(rule => rule.Order(x, y));
    }
}