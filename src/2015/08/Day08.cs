namespace AdventOfCode._2015._08;

public class Day08 : AdventOfCodeBase<Day08>
{
    public override object? Solution1(string input)
        => (from line in input.AsLines()
            let lineLength = line.Length
            let memoryLength = lineLength == 2 
                ? 0
                : DataLength(line[1..^1])
            let diff = lineLength - memoryLength
            let _ = $"{lineLength,2} => {memoryLength,2} ({diff,2}): {line}".Dump()
            select diff).Sum();

    public override object? Solution2(string input)
        => (from line in input.AsLines()
            let lineLength = line.Length
            let encodedLength = Encoded(line).Length + 2
            let diff = encodedLength - lineLength
            let _ = $"{lineLength,2} => {encodedLength,2} ({diff,2}): {line}".Dump()
            select diff).Sum();

    static int DataLength(string data) =>
        data.Length -
        (from match in Regex.Matches(data, @"(\\""|\\x[\da-f]{2}|\\\\)")
         let length = match.Length
         let adjustment = match.Length - 1
         let _ = $"Removing {adjustment} to cover {match.Value}".Dump()
         select adjustment).Sum();

    static string Encoded(string data)
        => data.Replace("\\", "\\\\")
            .Replace("\"", "\\\"");

}