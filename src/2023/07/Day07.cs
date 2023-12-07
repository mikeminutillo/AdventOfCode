namespace AdventOfCode._2023._07;

using static Day07.Strength;

public class Day07 : AdventOfCodeBase<Day07>
{
    public override object? Solution1(string input)
        => input.AsLines()
                .Select(CamelHand.Parse)
                .OrderBy(h => h.Strength)
                .ThenBy(h => h.CardStrength("23456789TJQKA"))
                .Ranked()
                .Sum(x => x.item.Bid * x.rank);

    public override object? Solution2(string input)
        => input.AsLines()
                .Select(CamelHand.Parse)
                .OrderBy(x => x.JokersWildStrength)
                .ThenBy(h => h.CardStrength("J23456789TQKA"))
                .Ranked()
                .Sum(x => x.item.Bid * x.rank);

    record CamelHand(string Hand, int Bid)
    {
        public Strength JokersWildStrength
            => Hand == "JJJJJ" 
             ? Strength 
             : (from sub in Hand.Except(['J'])
                let newHand = Hand.Replace('J', sub)
                select new CamelHand(newHand, Bid).Strength).Max();

        public Strength Strength
            => (from c in Hand
                group c by c into g
                let count = g.Count()
                orderby count descending
                select count).ToArray() switch
                {
                    [5] => FiveOfAKind,
                    [4, 1] => FourOfAKind,
                    [3, 2] => FullHouse,
                    [3, ..] => ThreeOfAKind,
                    [2, 2, 1] => TwoPair,
                    [2, ..] => OnePair,
                    [1, ..] => HighCard,
                    _ => throw new Exception("Not a valid hand")
                };

        public int CardStrength(string ranks) =>
            Hand.Select(c => ranks.IndexOf(c))
                .Aggregate(1, (s, c) => s * ranks.Length + c);

        public static CamelHand Parse(string input)
            => new(input[..5], int.Parse(input[6..]));
    }

    public enum Strength
    {
        HighCard,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        FullHouse,
        FourOfAKind,
        FiveOfAKind
    }
}