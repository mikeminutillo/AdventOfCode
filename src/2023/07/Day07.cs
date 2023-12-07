namespace AdventOfCode._2023._07;

public class Day07 : AdventOfCodeBase<Day07>
{
    public override object? Solution1(string input)
        => Ranked(input.AsLines().Select(CamelHand.Parse))
            .Sum(x => x.hand.Bid * x.rank);

    public override object? Solution2(string input)
        => RankedJokersWild(input.AsLines().Select(CamelHand.Parse))
            .Sum(x => x.hand.Bid * x.rank);


    static IEnumerable<(CamelHand hand, int rank)> Ranked(IEnumerable<CamelHand> hands)
            => hands.OrderBy(h => h.Strength)
                    .ThenBy(h => h.CardStrength("23456789TJQKA"))
                    .Select((hand, index) => (hand, index + 1));

    static IEnumerable<(CamelHand hand, int rank)> RankedJokersWild(IEnumerable<CamelHand> hands)
            => hands.OrderBy(x => x.JokersWildStrength("123456789TQKA"))
                    .ThenBy(h => h.CardStrength("J23456789TQKA"))
                    .Select((hand, Index) => (hand, Index + 1));

    record CamelHand(string Hand, int Bid)
    {
        public Strength JokersWildStrength(string substitutions)
            => (from sub in substitutions
                let newHand = Hand.Replace('J', sub)
                select new CamelHand(newHand, Bid).Strength).Max();

        public Strength Strength
            => (from c in Hand
                group c by c into g
                let count = g.Count()
                orderby count descending
                select count).ToArray() switch
                {
                    [5] => Strength.FiveOfAKind,
                    [4, 1] => Strength.FourOfAKind,
                    [3, 2] => Strength.FullHouse,
                    [3, ..] => Strength.ThreeOfAKind,
                    [2, 2, 1] => Strength.TwoPair,
                    [2, ..] => Strength.OnePair,
                    [1, ..] => Strength.HighCard,
                    _ => throw new Exception("Not a valid hand")
                };

        public int CardStrength(string ranks) =>
            Hand.Select(c => ranks.IndexOf(c))
                .Aggregate(1, (s, c) => s * ranks.Length + c);

        public static CamelHand Parse(string input)
            => new(input[..5], int.Parse(input[6..]));
    }

    enum Strength
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