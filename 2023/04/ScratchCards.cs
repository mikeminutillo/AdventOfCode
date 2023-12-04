using NUnit.Framework;
using System.Collections.Immutable;

namespace AdventOfCode._2023._04;

class ScratchCards : AdventOfCodeBase
{
    [TestCase]
    public void Example()
    {
        var input = Load("sample");

        var cards = GetCards(input);

        var result = cards.Sum(x => x.PrizeValue);

        Approve(result);
    }

    [TestCase]
    public void Example2()
    {
        var input = Load("sample");

        var cards = GetCards(input).ToArray();

        var result = AggregateCards(cards);

        Approve(result);
    }

    [TestCase]
    public void Problem1()
    {
        var input = Load("input");

        var cards = GetCards(input);

        var result = cards.Sum(x => x.PrizeValue);

        Approve(result);
    }

    [TestCase]
    public void Problem2()
    {
        var input = Load("input");

        var cards = GetCards(input).ToArray();

        var result = AggregateCards(cards);

        Approve(result);
    }

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
                        select section.ExtractNumbers().ToArray()
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
