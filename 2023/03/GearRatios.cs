using System.Drawing;
using System.Text.RegularExpressions;

namespace AdventOfCode._2023._03;

class GearRatios : AdventOfCodeBase<GearRatios>
{
    public override object Solution1(string input)
    {
        var lines = input.AsLines();
        var symParts = GetParts(lines, @"[^\d.]").ToArray();
        var numParts = GetParts(lines, @"\d+").ToArray();

        var result = GetAdjacentParts(numParts, symParts)
                        .Sum(p => p.Number);
        return result;
    }

    public override object Solution2(string input)
    {
        var lines = input.AsLines();

        var gearParts = GetParts(lines, @"\*").ToArray();
        var numParts = GetParts(lines, @"\d+").ToArray();

        var gearRatios = GetGearRatios(gearParts, numParts);

        return gearRatios.Sum();
    }

    IEnumerable <Part> GetParts(string[] input, string regex)
        => from i in Enumerable.Range(0, input.Length)
           from match in Regex.Matches(input[i], regex)
           select new Part(match.Value, i, match.Index);

    IEnumerable<Part> GetAdjacentParts(IEnumerable<Part> source, IEnumerable<Part> check)
        => from s in source
           let isAdjacent = check.Any(p => p.IsAdjacentTo(s))
           let _ = $"Line {s.Row + 1}: {(isAdjacent ? "  VALID" : "INVALID")} {s.Text} ({s.Col})".Dump()
           where isAdjacent
           select s;

    IEnumerable<int> GetGearRatios(IEnumerable<Part> gears, IEnumerable<Part> parts)
        => from g in gears
           let adjacentNums = parts.Where(g.IsAdjacentTo).ToArray()
           let isValid = adjacentNums.Length == 2
           let _ = $"Line {g.Row + 1}: {(isValid ? "  VALID" : "INVALID")} gear ({g.Col})".Dump()
           where isValid
           let ratio = adjacentNums.Product(x => x.Number)
           select ratio;

    record Part(string Text, int Row, int Col)
    {
        Rectangle Aura(int strength = 0)
            => new Rectangle(
                Col - strength,
                Row - strength,
                Text.Length + 2 * strength,
                1 + 2 * strength);

        public bool IsAdjacentTo(Part other)
            => Aura(1).IntersectsWith(other.Aura());
  
        public int Number => int.Parse(Text);
    }
}
