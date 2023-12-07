using System.Collections.Immutable;

namespace AdventOfCode._2015._03;

public class Day03 : AdventOfCodeBase<Day03>
{
    public override object? Solution1(string input)
        => input.Aggregate(DeliveryData.Start, (data, move) => data.Move(move))
                .History
                .Count;

    public override object? Solution2(string input)
        => ImmutableHashSet.Create<Point>()
        .Union(
            // Santa - every even step
            input.Where((c, i) => i % 2 == 0)
                 .Aggregate(DeliveryData.Start, (data, move) => data.Move(move))
                 .History
        )
        .Union(
            // Robo-Santa - every odd step (which is every even step if you skip 1)
            input.Skip(1)
                 .Where((c, i) => i % 2 == 0)
                 .Aggregate(DeliveryData.Start, (data, move) => data.Move(move))
                 .History
        ).Count;

    record Point(int X, int Y)
    {
        public Point Move(char c)
            => c switch
            {
                '^' => new(X, Y - 1),
                'v' => new(X, Y + 1),
                '<' => new(X - 1, Y),
                '>' => new(X + 1, Y),
                _ => this
            };

        public static readonly Point Zero = new(0, 0);
    }

    record DeliveryData(Point Location, ImmutableHashSet<Point> History)
    {
        public DeliveryData Move(char c)
            => MoveTo(Location.Move(c));

        DeliveryData MoveTo(Point newLocation)
            => new(newLocation, History.Add(newLocation));

        public static DeliveryData Start
            => new(Point.Zero, [Point.Zero]);
    }
}