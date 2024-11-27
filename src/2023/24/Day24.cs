namespace AdventOfCode._2023._24;

public class Day24 : AdventOfCodeBase<Day24>
{
    public override object? Solution1(string input)
        => input.AsLines()
                .Select(Hailstone3.Parse)
                .ToArray() switch
        {
            var stones => 
                (
                    from s1 in stones
                    from s2 in stones.SkipWhile(s => s != s1).Skip(1)
                    let intersection = s1.IntersectXY(s2)
                    let limited = stones.Length == 5
                        ? intersection.Limit(7, 27)
                        : intersection.Limit(
                            200000000000000,
                            400000000000000
                        )
                    select limited.Dump()
                )
                .Count(x => x.Hits)
        };

    record Intersection2D(Hailstone3 A, Hailstone3 B, string Text, bool Hits, Vector2? IntersectionPoint = null)
    {
        public override string ToString() 
            => $"""
                Hailstone A: {A}
                Hailstone B: {B}
                {Text}

                """;

        public Intersection2D Limit(decimal min, decimal max)
            => Hits
                ? IntersectionPoint! switch
                {
                    var p => p.X >= min && p.X <= max
                    && p.Y >= min && p.Y <= max
                        ? this with
                        {
                            Text = $"Hailstones' paths will cross inside the test area (at {p})."
                        }
                        : this with
                        {
                            Text = $"Hailstones' paths will cross outside the test area (at {p}).",
                            Hits = false
                        }
                }
                : this;

        public static Intersection2D Create(Hailstone3 A, Hailstone3 B, Func<Vector3, Vector2> projection)
            => (A.Project(projection), B.Project(projection)) switch
            {
                var (a2d, b2d) => GetIntersection(a2d, b2d) switch
                {
                    null => new(A, B, "Hailstones' paths are parallel; they never intersect.", false),
                    var p => (a2d.InFuture(p), b2d.InFuture(p)) switch
                    {
                        (true, true) => new(A, B, $"Hailstones' paths will cross (at {p}).", true, p),
                        (true, false) => new(A, B, "Hailstones' paths crossed in the past for hailstone B.", false),
                        (false, true) => new(A, B, "Hailstones' paths crossed in the past for hailstone A.", false),
                        (false, false) => new(A, B, "Hailstones' paths crossed in past for both hailstones.", false)
                    }
                }
            };

        static Vector2? GetIntersection(Hailstone2 a, Hailstone2 b)
            => (a.Velocity.X * b.Velocity.Y - a.Velocity.Y * b.Velocity.X) switch
            {
                0 => null,
                var determinant => (
                    a.Velocity.X * a.Position.Y - a.Velocity.Y * a.Position.X,
                    b.Velocity.X * b.Position.Y - b.Velocity.Y * b.Position.X
                ) switch
                {
                    var (b0, b1) => new(
                        (b.Velocity.X * b0 - a.Velocity.X * b1) / determinant,
                        (b.Velocity.Y * b0 - a.Velocity.Y * b1) / determinant
                    )
                }
            };
    }


    record Vector2(decimal X, decimal Y)
    {
        public override string ToString()
            => $"{X:0.###}, {Y:0.###}";
    }

    record Vector3(decimal X, decimal Y, decimal Z)
    {
        public override string ToString()
            => $"{X}, {Y}, {Z}";
    }

    record Hailstone2(Vector2 Position, Vector2 Velocity)
    {
        public override string ToString()
            => $"{Position} @ {Velocity}";

        public bool InFuture(Vector2 point)
            => Math.Sign(point.X - Position.X) == Math.Sign(Velocity.X);
    }

    record Hailstone3(Vector3 Position, Vector3 Velocity)
    {
        public static Hailstone3 Parse(string line)
            => line.Split('@', ',').Select(decimal.Parse).ToArray() switch
            {
                var v => new(
                    new(v[0], v[1], v[2]),
                    new(v[3], v[4], v[5])
                )
            };

        public override string ToString()
            => $"{Position} @ {Velocity}";

        public Hailstone2 Project(Func<Vector3, Vector2> projection)
            => new(projection(Position), projection(Velocity));

        public Intersection2D IntersectXY(Hailstone3 B)
            => Intersection2D.Create(this, B, v => new(v.X, v.Y));
    }
}