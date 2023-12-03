using NUnit.Framework;
using System.Text.RegularExpressions;

namespace AdventOfCode._2023._02;

class CubeConundrum : AdventOfCodeBase
{
    [TestCase]
    public void Example1()
    {
        var input = Load("sample1");

        var games = input.AsLines().Select(x => new Game(x));

        var totals = new Dictionary<string, int>
        {
            ["red"] = 12,
            ["green"] = 13,
            ["blue"] = 14
        };

        var validGames = games.Where(x => x.IsValid(totals));

        var validSum = validGames.Sum(x => x.GameNumber);

        Approve(validSum);
    }

    [TestCase] public void Example2()
    {
        var input = Load("sample1");

        var games = input.AsLines().Select(x => new Game(x));

        var result = games.Sum(x => x.Power);

        Approve(result);
    }

    [TestCase]
    public void Problem1()
    {
        var input = Load("input");

        var games = input.AsLines().Select(x => new Game(x));

        var totals = new Dictionary<string, int>
        {
            ["red"] = 12,
            ["green"] = 13,
            ["blue"] = 14
        };

        var validGames = games.Where(x => x.IsValid(totals));

        var validSum = validGames.Sum(x => x.GameNumber);

        Approve(validSum);
    }

    [TestCase]
    public void Problem2()
    {
        var input = Load("input");

        var games = input.AsLines().Select(x => new Game(x));

        var result = games.Sum(x => x.Power);

        Approve(result);
    }



    class Game
    {
        public int GameNumber { get; }

        Dictionary<string, int> pulls = new();

        public int Power => pulls.Values.Product();

        public bool IsValid(Dictionary<string, int> totals)
        {
            foreach(var total in totals)
            {
                if(!pulls.TryGetValue(total.Key, out var count))
                {
                    $"Game {GameNumber} is missing key {total.Key}".Dump();
                    return false;
                }

                if(total.Value < count)
                {
                    $"Game {GameNumber} has more {total.Key} ({count}) than the number in the bag ({total.Value})".Dump();
                    return false;   
                }
            }

            $"Game {GameNumber} is possible".Dump();

            return true;
        }

        public Game(string input)
        {
            var match = Regex.Match(input, @"Game (\d+): (.*)");
            GameNumber = int.Parse(match.Result("$1"));

            var results = match.Result("$2").Split(';');

            foreach(var result in results)
            {
                var match2 = Regex.Matches(result, @"(\d+) (red|blue|green)");
                foreach(Match r in match2)
                {
                    var count = int.Parse(r.Result("$1"));
                    var color = r.Result("$2");

                    if(pulls.TryGetValue(color, out var oldMax))
                    {
                        pulls[color] = Math.Max(oldMax, count);
                    }
                    else
                    {
                        pulls[color] = count;
                    }
                }
            }
        }
    }
}
