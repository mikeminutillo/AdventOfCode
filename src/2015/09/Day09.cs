using System.Collections.Immutable;

namespace AdventOfCode._2015._09;

public class Day09 : AdventOfCodeBase<Day09>
{
    public override object? Solution1(string input)
        => Graph.Parse(input).ShortestJourney()?.DistanceTravelled;

    record Graph(ImmutableArray<Route> Edges, ImmutableHashSet<string> Locations)
    {
        public static Graph Parse(string input)
            => Create(input.AsLines()
                        .Select(Route.Parse)
                        .SelectMany(x => new[] { x, x.Reverse })
                        .ToImmutableArray());

        static Graph Create(ImmutableArray<Route> edges)
            => new(edges, edges.Aggregate(ImmutableHashSet.Create<string>(),
                (set, route) => set.Union([route.From, route.To])));

        IEnumerable<Route> GetExits(string location)
            => Edges.Where(x => x.From == location);

        public Journey? ShortestJourney()
            => GetShortestJourney([.. Edges.Select(Journey.Start).OrderBy(x => x.DistanceTravelled)]);

        Journey? GetShortestJourney(ImmutableArray<Journey> trails)
            => trails.Length is 0
            ? null
            : CheckAndExtendTrail(trails.RemoveAt(0), trails[0]);

        Journey? CheckAndExtendTrail(ImmutableArray<Journey> trails, Journey trail)
            => Locations.Except(trail.LocationsCovered) is { IsEmpty: false } remaining
            ? GetShortestJourney(
                [.. trails.AddRange(
                        GetExits(trail.CurrentLocation)
                            .Where(x => remaining.Contains(x.To))
                            .Select(trail.Add)
                    ).OrderBy(x => x.DistanceTravelled)

                ])
            : trail;

    }

    record Journey(ImmutableArray<Route> Steps)
    {
        public int DistanceTravelled
            => Steps.Sum(x => x.Distance);

        public string CurrentLocation
            => Steps.Last().To;

        public Journey Add(Route route)
            => new(Steps.Add(route));

        public ImmutableHashSet<string> LocationsCovered
            => Steps.Aggregate(ImmutableHashSet.Create<string>(),
                (set, route) => set.Union([route.From, route.To]));

        public int MaxLocationRepeats => Steps.SelectMany(x => new[] { (location: x.To, count: 1), (location: x.From, count: 1) })
            .GroupBy(x => x.location, x => x.count)
            .Select(x => x.Sum())
            .Max();

        public static Journey Start(Route firstStep)
            => new([firstStep]);

        public override string ToString()
            => $"{DistanceTravelled, 3}: {Steps[0].From} -> {string.Join(" -> ", Steps.Select(x => x.To))}";
    }

    record Route(string From, string To, int Distance)
    {
        public static Route Parse(string line)
            => Regex.Match(line, @"(.*) to (.*) = (\d+)") is {  Success: true } match
            ? new(match.Result("$1"), match.Result("$2"), int.Parse(match.Result("$3")))
            : throw new Exception($"Unreadable route {line}");

        public Route Reverse => new(To, From, Distance);
    }
}
