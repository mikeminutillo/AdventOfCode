using System.Collections.Immutable;
using System.Numerics;

namespace AdventOfCode._2025._05;

public class Day05 : AdventOfCodeBase<Day05>
{
    public override object? Solution1(string input)
        => Database.Parse(input).GetAllFreshIngredients().Count();

    public override object? Solution2(string input)
        => Database.Parse(input).Normalized().FreshRanges.Sum(x => x.ItemsCovered);

    record Range<T>(T Min, T Max) where T : INumber<T>
    {
        public T ItemsCovered => Max - Min + T.One;

        public ImmutableArray<Range<T>> Union(Range<T> other)
            => (Contains(other.Min), Contains(other.Max)) switch
            {
                (true, true) => [this],
                (true, false) => [new(Min, other.Max)],
                (false, true) => [new(other.Min, Max)],
                (false, false) => [this, other]
            };

        public bool Intersects(Range<T> other)
            => Contains(other.Min) || Contains(other.Max)
            || other.Contains(Min) || other.Contains(Max);

        public bool Contains(T id)
            => id >= Min && id <= Max;

        public static ImmutableArray<Range<T>> Normalize(ImmutableArray<Range<T>> input)
            => input switch
            {
                [] => [],
                [var x] => [x],
                [var x, var y] => x.Union(y),
                [var head, .. var tail] => tail.FirstOrDefault(x => x.Intersects(head)) switch
                {
                    null => [head, ..Normalize(tail)],
                    var match => Normalize([..match.Union(head), ..tail.Remove(match)])
                }
            };
    }

    record Database(ImmutableArray<Range<decimal>> FreshRanges, ImmutableArray<decimal> AvailableIngredients)
    {
        public Database Normalized() => new(Range<decimal>.Normalize(FreshRanges), AvailableIngredients);

        public IEnumerable<decimal> GetAllFreshIngredients()
            => from ingredient in AvailableIngredients
               where FreshRanges.Any(range => range.Contains(ingredient))
               select ingredient;

        public static Database Parse(string input)
            => input.AsLines() switch
            {
                var lines => lines
                    .Where(x => string.IsNullOrWhiteSpace(x) == false)
                    .Aggregate(
                        (
                            fresh: ImmutableArray<Range<decimal>>.Empty,
                            available: ImmutableArray<decimal>.Empty
                        ), 
                        (state, line) => line.ExtractNumbers<decimal>().ToArray() switch
                        {
                            [var one] => (state.fresh, state.available.Add(one)),
                            [var min, var max] => (

                                state.fresh.Add(new(min, max)), 
                                state.available
                            ),
                            _ => throw new Exception("Invalid input")
                        },
                        state => new Database(
                            state.fresh,
                            state.available
                        )
                    )
            };
    }
}