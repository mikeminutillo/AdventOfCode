using System.Collections.Immutable;

namespace AdventOfCode._2023._04;

class Day04 : AdventOfCodeBase<Day04>
{
    public override object Solution1(string input)
        => GetCards(input)
        .Sum(x => x.PrizeValue);

    public override object Solution2(string input)
        => AggregateCards(GetCards(input).ToArray());

    int AggregateCards(IEnumerable<ScratchCard> cards)
        => cards.Aggregate(
            cards.ToImmutableDictionary(x => x.CardNumber, x => 1),
            (counts, card) => counts.SetItems(
                from bonus in card.PrizeCards 
                select KeyValuePair.Create(
                    bonus,
                    counts[bonus] + counts[card.CardNumber])
                )
           ).Values.Sum();

    IEnumerable<ScratchCard> GetCards(string input)
        => from line in input.AsLines()
           let split = (from section in line.Split([.. ":|"])
                        select section.ExtractNumbers<int>().ToArray()
                       ).ToArray()
           let cardNumber = split[0].Single()
           let winningNumbers = split[1]
           let gameNumbers = split[2]
           select new ScratchCard(
               cardNumber,
               gameNumbers.Intersect(winningNumbers).Count()
            );

    record ScratchCard(int CardNumber, int WinningNumbers)
    {
        public int PrizeValue
            => WinningNumbers > 0
            ? (int)Math.Pow(2, WinningNumbers - 1)
            : 0;

        public IEnumerable<int> PrizeCards
            => Enumerable.Range(CardNumber + 1, WinningNumbers);
    }
}
