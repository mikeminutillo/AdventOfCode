
namespace AdventOfCode._2024._04;

public class Day04 : AdventOfCodeBase<Day04>
{

    public override object? Solution1(string input)
        => new WordSearch(input.AsLines())
            .FindWords("XMAS")
            .Select(x => x.Dump())
            .Count()
            .Dump();

    public override object? Solution2(string input)
        => new WordSearch(input.AsLines())
            .FindXWords("MAS")
            .Select(x => x.Dump())
            .Count()
            .Dump();

    record Vector(int X, int Y)
    {
        public static Vector operator +(Vector src, Vector add)
            => src with
            {
                X = src.X + add.X,
                Y = src.Y + add.Y,
            };

        public static Vector operator *(Vector src, int multiple)
            => src with
            {
                X = src.X * multiple,
                Y = src.Y * multiple,
            };
    }

    static readonly Vector Up = new(0, -1);
    static readonly Vector Down = new(0, 1);
    static readonly Vector Left = new(-1, 0);
    static readonly Vector Right = new(1, 0);

    static readonly Vector[] CompassDirections = [
        Up + Left,
        Left,
        Down + Left,
        Down,
        Down + Right,
        Right,
        Up + Right,
        Up
    ];

    record WordSearch(string[] Lines)
    {
        bool Contains(Vector location)
            => location.Y >= 0 && location.Y < Lines.Length
            && location.X >= 0 && location.X < Lines[location.Y].Length;

        char At(Vector location)
            => Lines[location.Y][location.X];

        public IEnumerable<(Vector location, Vector direction)> FindWords(string word)
            => from location in AllLocations()
               from direction in CompassDirections
               where Find(word, location, direction)
               select (location, direction);

        public IEnumerable<Vector> FindXWords(string word)
            => from location in AllLocations()
               where IsXWord(location, word)
               select location;

        IEnumerable<Vector> AllLocations()
            => from row in Enumerable.Range(0, Lines.Length)
               let line = Lines[row]
               from col in Enumerable.Range(0, line.Length)
               select new Vector(col, row);

        bool IsXWord(Vector location, string word)
            => (word.Length / 2) switch
            {
                var offset => 
                    At(location) == word[offset]
                    && (
                        Find(word, location + (Up + Left) * offset, Down + Right)
                     || Find(word, location + (Down + Right) * offset, Up + Left)
                    )
                    && (
                        Find(word, location + (Down + Left) * offset, Up + Right)
                     || Find(word, location + (Up + Right) * offset, Down + Left)
                    )
            };

        bool Find(ReadOnlySpan<char> letters, Vector location, Vector direction)
            => letters switch
            {
                [] => true,
                [var head, .. var tail] =>
                       Contains(location)
                    && At(location) == head
                    && Find(tail, location + direction, direction)
            };
    }
}