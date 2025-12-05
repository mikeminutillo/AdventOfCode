using System.Collections.Immutable;
using System.Text;

namespace AdventOfCode._2025._04;

public class Day04 : AdventOfCodeBase<Day04>
{
    public override object? Solution1(string input) => Map.Parse(input).GetRemovable(4).Count();//.Print(4);

    public override object? Solution2(string input) => Map.Parse(input).GetTotalRemovable(4).Count();
        
    record Map(ImmutableHashSet<Location> RollLocations)
    {
        public string Print(int? removableLimit = null)
        {
            var maxRow = RollLocations.Max(x => x.Row);
            var maxCol = RollLocations.Max(x => x.Col);
            ImmutableHashSet<Location> removable = removableLimit.HasValue ? [.. GetRemovable(removableLimit.Value)] : [];
            StringBuilder sb = new();

            for(var row = 0; row <= maxRow; row++)
            {
                for(var col = 0; col <= maxCol; col++)
                {
                    var pos = new Location(row, col);
                    if(removable.Contains(pos))
                    {
                        sb.Append('x');
                    } else if(RollLocations.Contains(pos))
                    {
                        sb.Append('@');
                    }
                    else
                    {
                        sb.Append('.');
                    }
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public IEnumerable<Location> GetRemovable(int limitToRemove)
            => from location in RollLocations.AsParallel()
               let count = location.GetAdjacencies().Intersect(RollLocations).Count()
               where count < limitToRemove
               select location;

        public static Map Parse(string input)
            => input.AsLines() switch
            {
                var lines => new Map([
                    ..from row in 0.UpTo(lines.Length - 1).AsParallel()
                      let line = lines[row]
                      from col in 0.UpTo(line.Length - 1)
                      where line[col] == '@'
                      select new Location(row, col)
                ])
            };

        public IEnumerable<Location> GetTotalRemovable(int limitToRemove)
            => GetRemovable(limitToRemove).ToArray() switch
            {
                [] => [],
                var removable => [
                    ..removable,
                    .. new Map(RollLocations.Except(removable))
                        .GetTotalRemovable(limitToRemove)
                ]
            };
    }

    record Location(int Row, int Col)
    {
        static readonly int[] Offsets = [-1, 0, 1];

        public IEnumerable<Location> GetAdjacencies()
            => from rowOffset in Offsets
               from colOffset in Offsets
               where (rowOffset, colOffset) is not (0, 0)
               select new Location(Row + rowOffset, Col + colOffset);
    }
}