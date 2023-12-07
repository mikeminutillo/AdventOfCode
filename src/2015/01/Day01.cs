namespace AdventOfCode._2015._01;

public class Day01 : AdventOfCodeBase<Day01>
{
    public override object? Solution1(string input)
        => Movements(input).Sum();

    public override object? Solution2(string input)
        => Floors(Movements(input)).TakeWhile(x => x >= 0).Count() + 1;

    
    IEnumerable<int> Floors(IEnumerable<int> movements)
    {
        var floor = 0;
        foreach (var movement in movements)
        {
            floor += movement;
            yield return floor;
        }
    }

    IEnumerable<int> Movements(string input)
        => from c in input
           select c switch
           {
               '(' => 1,
               ')' => -1,
               _ => throw new Exception("Unknown input")
           };
}