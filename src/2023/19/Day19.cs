using System.Collections.Immutable;

namespace AdventOfCode._2023._19;

public class Day19 : AdventOfCodeBase<Day19>
{
    public override object? Solution1(string input)
        => Parse(input) switch
        {
            (var parts, var engine) => (from part in parts.AsParallel()
                                        let result = engine.Process(part)
                                        let _ = $"{result}: {part}".Dump()
                                        select (result, part))
                            .Where(x => x.result == "A")
                            .Sum(p => p.part.Rating).Dump()
        };

    public override object? Solution2(string input)
        => Parse(input) switch
        {
            (_, var engine) => engine.Process(PotentialPart.Create())
                                    .Where(x => x.Result == "A")
                                    .Sum(x => x.Part.Count).Dump()
        };

    static (ImmutableArray<Part> Parts, WorkflowEngine Engine) Parse(string input)
        => input.Split("\n\n") switch
        {
        [var top, var bottom] => (
                   bottom.AsLines()
                         .Select(Part.Parse)
                         .ToImmutableArray(),
                    WorkflowEngine.Parse(top)
        )
        };


    record WorkflowEngine(ImmutableDictionary<string, ImmutableArray<WorkflowStep>> Workflows)
    {
        public string Process(Part part, string workflow = "in")
            => !Workflows.TryGetValue(workflow, out var chain)
            ? workflow
            : chain.Select(x => x.Process(part))
                    .First(x => x != null) switch
            {
                var nextStep => Process(part, nextStep!)
            };


        public ImmutableArray<(string Result, PotentialPart Part)> Process(PotentialPart part, string workflow = "in")
            => !Workflows.TryGetValue(workflow, out var chain)
            ? [(workflow, part)]
            : chain.Aggregate(
                (
                    Results: ImmutableArray<(string Result, PotentialPart Part)>.Empty,
                    CurrentPart: (PotentialPart?)part
                ),
                (state, step) => state.CurrentPart is null
                    ? state
                    : step.Process(state.CurrentPart) switch
                    {
                        (var label, var matched, var unmatched) => matched is null
                            ? state
                            : (state.Results.Add((label, matched!)), unmatched)
                    }).Results switch
            {
                var transformedChain => (from transformed in transformedChain
                                         from result in Process(transformed.Part, transformed.Result)
                                         select result).ToImmutableArray()
            };

        public static WorkflowEngine Parse(string input)
            => input.AsLines() switch
            {
                var lines => new(
                    lines.Aggregate(
                        ImmutableDictionary<string, ImmutableArray<WorkflowStep>>.Empty,
                        (state, line) => line.Split(new[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries) switch
                        {
                        [var label, var stepsBlock] => stepsBlock.Split(',') switch
                        {
                            var steps => (from step in steps
                                          select Regex.Match(step, @"(\w+)([<>])(\d+):(\w+)") is { Success: true } match
                                          ? (match.Result("$1"), match.Result("$2"), long.Parse(match.Result("$3")), match.Result("$4")) switch
                                          {
                                              (var attribute, "<", var threshold, var onSuccess) => new LessThanWorkflowStep(attribute, threshold, onSuccess),
                                              (var attribute, ">", var threshold, var onSuccess) => new GreaterThanWorkflowStep(attribute, threshold, onSuccess),
                                          }
                                          : Regex.Match(step, @"(\w+)") is { Success: true } match2
                                          ? (WorkflowStep)new JumpWorkflowStep(match2.Result("$1"))
                                          : throw new Exception($"Unknown workflow step {step}")
                                          ).ToImmutableArray() switch
                            {
                                var stepsArray => state.SetItem(label, stepsArray)
                            }
                        }
                        })
                    )
            };
    }


    interface WorkflowStep
    {
        string? Process(Part part);
        (string Label, PotentialPart? Matched, PotentialPart? Unmatched) Process(PotentialPart part);
    }

    record JumpWorkflowStep(string Label) : WorkflowStep
    {
        public string? Process(Part part)
            => Label;

        (string Label, PotentialPart? Matched, PotentialPart? Unmatched) WorkflowStep.Process(PotentialPart part)
            => (Label, part, null);
    }

    record LessThanWorkflowStep(string Attribute, long Threshold, string OnSuccess) : WorkflowStep
    {
        public string? Process(Part part)
            => part.Attributes[Attribute] < Threshold
                ? OnSuccess
            : null;

        public (string Label, PotentialPart? Matched, PotentialPart? Unmatched) Process(PotentialPart part)
            => part.Split(Attribute, Threshold - 1) switch
            {
                (var lower, var upper) => (OnSuccess, lower, upper)
            };
    }

    record GreaterThanWorkflowStep(string Attribute, long Threshold, string OnSuccess) : WorkflowStep
    {
        public string? Process(Part part)
            => part.Attributes[Attribute] > Threshold
                ? OnSuccess
                : null;

        public (string Label, PotentialPart? Matched, PotentialPart? Unmatched) Process(PotentialPart part)
            => part.Split(Attribute, Threshold) switch
            {
                (var lower, var upper) => (OnSuccess, upper, lower)
            };
    }

    record Range(long Lower, long Upper)
    {
        public ImmutableArray<Range> Split(long point)
            => [new(Lower, point), new(point + 1, Upper)];

        public bool IsValid
            => Lower <= Upper;

        public long Length
            => Upper - Lower + 1;
    }

    record PotentialPart(ImmutableDictionary<string, Range> Attributes)
    {
        public static PotentialPart Create()
            => new(
                "xmas".Aggregate(
                    ImmutableDictionary<string, Range>.Empty,
                    (state, c) => state.SetItem($"{c}", new Range(1, 4000))
                ));

        public (PotentialPart? Lower, PotentialPart? Upper) Split(string attribute, long slice)
            => Attributes[attribute].Split(slice) switch
            {
            [var lower, var upper] => (
                lower.IsValid ? new(Attributes.SetItem(attribute, lower)) : null,
                upper.IsValid ? new(Attributes.SetItem(attribute, upper)) : null
            )
            };

        public long Count
            => Attributes.Values.Aggregate(1L, (p, a) => a.Length * p);
    }

    record Part(ImmutableDictionary<string, long> Attributes)
    {
        public static Part Parse(string line)
            => new(
                Regex.Matches(line, @"([xmas])=(\d+)")
                     .Cast<Match>()
                     .Aggregate(
                          ImmutableDictionary.Create<string, long>(),
                          (state, match) => state.SetItem(match.Result("$1"), long.Parse(match.Result("$2")))
                    )
                );

        public long Rating
            => Attributes["x"]
             + Attributes["m"]
             + Attributes["a"]
             + Attributes["s"];

        public override string ToString()
            => $"{{x={Attributes["x"]},m={Attributes["m"]},a={Attributes["a"]},s={Attributes["s"]}}} :: {Rating}";

    }
}