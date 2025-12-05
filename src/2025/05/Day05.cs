using System.Collections.Immutable;

namespace AdventOfCode._2025._05;

public class Day05 : AdventOfCodeBase<Day05>
{
    public override object? Solution1(string input)
        => Database.Parse(input).GetAllFreshIngredients().Count();

    public override object? Solution2(string input)
        => Database.Parse(input).FreshRanges.Sum(x => x.ItemsCovered);
    
    record PartId(string Id)
    {
        public static bool operator >=(PartId left, PartId right)
            => (left.Id.Length - right.Id.Length) switch
            {
                < 0 => false,
                > 0 => true,
                0 => string.Compare(left.Id, right.Id, StringComparison.Ordinal) >= 0
            };

        public static bool operator <=(PartId left, PartId right)
            => (left.Id.Length - right.Id.Length) switch
            {
                < 0 => true,
                > 0 => false,
                0 => string.Compare(left.Id, right.Id, StringComparison.Ordinal) <= 0
            };

        public static decimal operator -(PartId left, PartId right)
            => decimal.Parse(left.Id) - decimal.Parse(right.Id);
    }

    record IngredientRange(PartId Min, PartId Max)
    {
        public decimal ItemsCovered => Max - Min + 1;

        public ImmutableArray<IngredientRange> Intersection(IngredientRange other)
            => (Contains(other.Min), Contains(other.Max)) switch
            {
                (true, true) => [this],
                (true, false) => [new(Min, other.Max)],
                (false, true) => [new(other.Min, Max)],
                (false, false) => [this, other]
            };

        public bool Intersects(IngredientRange other)
            => Contains(other.Min) || Contains(other.Max);

        public bool Contains(PartId id)
            => id >= Min && id <= Max;

        public static IngredientRange Parse(string input)
            => input.Split('-') switch
            {
                var parts => new IngredientRange(
                    new(parts[0]), 
                    new(parts[1])
                )
            };

        public static ImmutableArray<IngredientRange> Normalize(ImmutableArray<IngredientRange> input)
            => input switch
            {
                [] => [],
                [var x] => [x],
                [var x, var y] => x.Intersection(y),
                [var head, .. var tail] => tail.FirstOrDefault(x => x.Intersects(head) || head.Intersects(x)) switch
                {
                    null => [head, ..Normalize(tail)],
                    var match => Normalize([..match.Intersection(head), ..tail.Remove(match)])
                }
            };
    }

    record Database(ImmutableArray<IngredientRange> FreshRanges, ImmutableArray<PartId> AvailableIngredients)
    {
        public IEnumerable<PartId> GetAllFreshIngredients()
            => from ingredient in AvailableIngredients
               let matchingRanges = FreshRanges.Where(range => range.Contains(ingredient)).ToArray()
               where matchingRanges.Dump().Length != 0
               select ingredient.Dump();

        public static Database Parse(string input)
            => input.AsLines() switch
            {
                var lines => lines
                    .Where(x => string.IsNullOrWhiteSpace(x) == false)
                    .Aggregate(
                        (
                            fresh: ImmutableArray<IngredientRange>.Empty,
                            available: ImmutableArray<PartId>.Empty
                        ), 
                        (state, line) => line.Contains('-')
                            ? (state.fresh.Add(IngredientRange.Parse(line)), state.available)
                            : (state.fresh, state.available.Add(new(line))),
                        state => new Database(
                            IngredientRange.Normalize(state.fresh),
                            state.available
                        )
                    )
            };
    }
}