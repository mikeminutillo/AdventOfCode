namespace AdventOfCode._2015._05;

public class Day05 : AdventOfCodeBase<Day05>
{
    public override object? Solution1(string input)
        => Nice(input.AsLines()).Count();

    public override object? Solution2(string input)
        => BetterNice(input.AsLines()).Count();

    public IEnumerable<string> Nice(IEnumerable<string> input)
        => from line in input
           let vowels = from m in Regex.Matches(line, "[aeiou]") select m.Value
           let repeatingChars = from m in Regex.Matches(line, @"(.)\1") select m.Value
           let badSequences = from m in Regex.Matches(line, "(ab|cd|pq|xy)") select m.Value
           let vowelCount = vowels.Count()
           let repeatingCharCount = repeatingChars.Count()
           let badSequenceCount = badSequences.Count()
           let isNice = vowelCount >= 3
            && repeatingCharCount > 0
           && badSequenceCount == 0
           let _ = $"{(isNice ? "   NICE" : "NAUGHTY")}: {line} ({vowelCount}, {repeatingCharCount}, {badSequenceCount})".Dump()
           where isNice
           select line;

    public IEnumerable<string> BetterNice(IEnumerable<string> input)
        => from line in input
           let repeatingSequences = Regex.Matches(line, @"(..).*\1")
           let mirrorSequences = Regex.Matches(line, @"(.).\1")
           where repeatingSequences.Any()
           && mirrorSequences.Any()
           select line;


}