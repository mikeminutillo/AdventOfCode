using System.Collections.Immutable;

namespace AdventOfCode._2023._22;

public class Day22 : AdventOfCodeBase<Day22>
{
    public override object? Solution1(string input)
        => input.AsLines()
                .Select(Box.Parse)
                .ToImmutableArray() switch
        {
            var boxes => Fall(boxes) switch
            {
                var fallenBoxes => RemovableBoxes(fallenBoxes).Dump()
            }
        };

    public override object? Solution2(string input) 
        => input.AsLines()
                .Select(Box.Parse)
                .ToImmutableArray() switch
        {
            var boxes => Fall(boxes) switch
            {
                var fallenBoxes => CountFallingBoxes(fallenBoxes).Dump()
            }
        };

    int RemovableBoxes(ImmutableArray<Box> boxes)
    {
        var boxPoints = boxes.ToDictionary(
            b => b,
            b => b.GetPoints().ToImmutableHashSet());
        var downPoints = boxes.ToDictionary(
            b => b,
            b => b.GetPoints().Select(p => p.Down()).ToImmutableHashSet());

        var count = 0;
        foreach (var box in boxes)
        {
            var canDestroy = true;
            var spaces = boxPoints[box];
            foreach (var other in boxes)
            {
                var otherSupportPoints = downPoints[other];
                if (other != box && !otherSupportPoints.Intersect(spaces).IsEmpty)
                {
                    $"{other} is supported by {box}. Seeing if it is otherwise supported".Dump();
                    // other is supported by this box, is it supported by anything else?
                    var otherwiseSupported = false;
                    foreach (var thirdBox in boxes)
                    {
                        if (thirdBox != box && thirdBox != other)
                        {
                            var thirdBoxPoints = boxPoints[thirdBox];
                            if (!otherSupportPoints.Intersect(thirdBoxPoints).IsEmpty)
                            {
                                $"{other} is supported by {thirdBox}".Dump();
                                otherwiseSupported = true;
                            }
                        }
                    }
                    if (!otherwiseSupported)
                    {
                        $"Cannot destroy {box} or {other} will fall".Dump();
                        canDestroy = false;
                    }
                }
            }
            if (canDestroy)
            {
                $"CAN DESTROY {box} * * * * * * * * * * * * * * * *".Dump();
                count++;
            }
            else
            {
                $"CANNOT DESTROY {box} * * * * * * * * * * * * * * *".Dump();
            }
        }
        return count;
    }

    ImmutableArray<Box> Fall(ImmutableArray<Box> boxes)
    {
        Dictionary<Point, int> stack = [];
        List<Box> fallen = [];

        var q = new PriorityQueue<Box, int>();
        foreach (var box in boxes)
        {
            q.Enqueue(box, box.LowestPoint);
        }

        while (q.TryDequeue(out var box, out var originalHeight))
        {
            var shadow = box.GetShadow();
            var stackRoof = shadow.Max(s => stack.TryGetValue(s.Key, out var h)
                                ? h
                                : 0);

            var newHeight = stackRoof + 1;

            var drop = originalHeight - newHeight;

            var newLocation = box.Drop(drop);

            $"Dropped ({box}) by {drop} to ({newLocation})".Dump();

            fallen.Add(newLocation);

            foreach (var space in shadow.Keys)
            {
                stack[space] = newHeight + shadow[space] - 1;
            }
        }
        return [.. fallen];
    }

    int CountFallingBoxes(ImmutableArray<Box> boxes)
    {
        var boxPoints = boxes.ToDictionary(
            b => b,
            b => b.GetPoints().ToImmutableHashSet());

        var supports = new List<(Box supporting, Box supported)>();

        foreach (var box in boxes)
        {
            var points = boxPoints[box];
            var directlyAbove = points.Select(x => x.Up()).ToImmutableHashSet();
            //var directlyBelow = points.Select(x => x.Down()).ToImmutableHashSet();

            foreach (var other in boxes)
            {
                if (other != box)
                {
                    if (!directlyAbove.Intersect(boxPoints[other]).IsEmpty)
                    {
                        supports.Add((box, other));
                    }
                }
            }
        }

        return (from box in boxes.AsParallel()
                let howMany = HowManyFall([box], [.. supports])
                let _ = $"Removing {box} causes {howMany} to fall".Dump()
                select howMany).Sum();

    }

    int HowManyFall(ImmutableHashSet<Box> toRemove, ImmutableArray<(Box supporting, Box supported)> supports)
        => supports.Where(s => toRemove.Contains(s.supporting))
                   .Select(s => s.supported)
                   .ToImmutableHashSet() switch
        {
            var potentialFalling => potentialFalling.IsEmpty
                ? 0
                : (from maybeFalling in potentialFalling
                   let supportingBoxes = supports.Where(s => s.supported == maybeFalling).Select(s => s.supporting).ToImmutableHashSet()
                   let remainingSupports = supportingBoxes.Except(toRemove)
                   where remainingSupports.IsEmpty
                   select maybeFalling
                   ).ToImmutableHashSet() switch
                {
                    var falling => falling.Count
                        + HowManyFall(falling, supports.Where(s => !toRemove.Contains(s.supporting)).ToImmutableArray())
                }
        };
    
    record Point(int X, int Y)
    {

    }


    record Point3d(int X, int Y, int Z)
    {
        public static Point3d Parse(string text)
            => text.Split(',').Select(int.Parse).ToArray() switch
            {
            [var x, var y, var z] => new(x, y, z)
            };

        public Point3d Up()
            => new(X, Y, Z + 1);

        public Point3d Down()
            => new(X, Y, Z - 1);

        public override string ToString()
            => $"{X},{Y},{Z}";
    }

    record Box(Point3d FirstEnd, Point3d SecondEnd)
    {
        public static Box Parse(string text)
            => text.Split('~') switch
            {
            [var a, var b] => new(Point3d.Parse(a), Point3d.Parse(b))
            };

        public int LowestPoint
            => Math.Min(FirstEnd.Z, SecondEnd.Z);

        public ImmutableDictionary<Point, int> GetShadow()
            => Math.Abs(FirstEnd.X - SecondEnd.X) switch
            {
                0 => Math.Abs(FirstEnd.Y - SecondEnd.Y) switch
                {
                    0 => Math.Abs(FirstEnd.Z - SecondEnd.Z) switch
                    {
                        var height => Enumerable.Range(0, 1)
                                                .ToImmutableDictionary(
                                                    _ => new Point(FirstEnd.X, FirstEnd.Y),
                                                    _ => height + 1)
                    },
                    var breadth => Enumerable.Range(0, breadth + 1)
                                             .Select(yOff => Math.Min(FirstEnd.Y, SecondEnd.Y) + yOff)
                                             .ToImmutableDictionary(
                                                    y => new Point(FirstEnd.X, y),
                                                    _ => 1)
                },
                var width => Enumerable.Range(0, width + 1)
                                       .Select(xOff => Math.Min(FirstEnd.X, SecondEnd.X) + xOff)
                                       .ToImmutableDictionary(
                                            x => new Point(x, FirstEnd.Y),
                                            _ => 1)
            };

        public ImmutableArray<Point3d> GetPoints()
            => FirstEnd.X != SecondEnd.X
            ? Enumerable.Range(Math.Min(FirstEnd.X, SecondEnd.X), Math.Abs(FirstEnd.X - SecondEnd.X) + 1)
                        .Select(x => new Point3d(x, FirstEnd.Y, FirstEnd.Z))
                        .ToImmutableArray()
            : FirstEnd.Y != SecondEnd.Y
            ? Enumerable.Range(Math.Min(FirstEnd.Y, SecondEnd.Y), Math.Abs(FirstEnd.Y - SecondEnd.Y) + 1)
                        .Select(y => new Point3d(FirstEnd.X, y, FirstEnd.Z))
                        .ToImmutableArray()
            : Enumerable.Range(Math.Min(FirstEnd.Z, SecondEnd.Z), Math.Abs(FirstEnd.Z - SecondEnd.Z) + 1)
                        .Select(z => new Point3d(FirstEnd.X, FirstEnd.Y, z))
                        .ToImmutableArray();

        public override string ToString()
            => $"{FirstEnd}~{SecondEnd}";

        public Box Drop(int drop)
            => new(new(FirstEnd.X, FirstEnd.Y, FirstEnd.Z - drop),
                new(SecondEnd.X, SecondEnd.Y, SecondEnd.Z - drop));
    }
}