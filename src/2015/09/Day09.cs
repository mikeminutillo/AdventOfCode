using System.Collections.Immutable;

namespace AdventOfCode._2015._09;

public class Day09 : AdventOfCodeBase<Day09>
{
    public override object? Solution1(string input)
        => Graph.Parse(input).GetAllCoveringJourneys()
            .OrderBy(x => x.DistanceTravelled)
            .First().Dump()
            .DistanceTravelled;

    public override object? Solution2(string input)
        => Graph.Parse(input).GetAllCoveringJourneys()
            .OrderByDescending(x => x.DistanceTravelled)
            .First().Dump()
            .DistanceTravelled;
    
    record Graph(ImmutableArray<Route> Edges)
    {
        public static Graph Parse(string input)
            => new(input.AsLines()
                        .Select(Route.Parse)
                        .SelectMany(x => new[] { x, x.Reverse })
                        .ToImmutableArray());

        ImmutableHashSet<string> GetLocationsExcept(params string[] exceptions)
            => Edges.Aggregate(ImmutableHashSet.Create<string>(),
                (set, route) => set.Union([route.From, route.To]))
                    .Except(exceptions);

        IEnumerable<Route> GetExits(string location)
            => Edges.Where(x => x.From == location);

        public IEnumerable<Journey> GetAllCoveringJourneys()
            => from edge in Edges
               from journey in GetAllCoveringJourneys(edge)
               select journey;

        IEnumerable<Journey> GetAllCoveringJourneys(Route firstStep)
            => GetAllCoveringJourneys(
                Journey.Start(firstStep),
                GetLocationsExcept(firstStep.From, firstStep.To)
            );

        IEnumerable<Journey> GetAllCoveringJourneys(Journey journey, ImmutableHashSet<string> toVisit)
            => toVisit.IsEmpty
            ? [journey.Dump()]
            : from exit in GetExits(journey.CurrentLocation)
              where toVisit.Contains(exit.To)
              from continuing in GetAllCoveringJourneys(journey.Add(exit), toVisit.Remove(exit.To))
              select continuing;
    }

    record Journey(ImmutableArray<Route> Steps)
    {
        public int DistanceTravelled
            => Steps.Sum(x => x.Distance);

        public string CurrentLocation
            => Steps[^1].To;

        public Journey Add(Route route)
            => new(Steps.Add(route));

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
