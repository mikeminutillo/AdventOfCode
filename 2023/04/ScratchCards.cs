using NUnit.Framework;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace AdventOfCode._2023._04;

class ScratchCards : AdventOfCodeBase
{
    [TestCase]
    public void Example()
    {
        var input = Load("sample");

        var cards = GetCards(input);

        var result = cards.Sum(x => x.Prize());

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

        var result = cards.Sum(x => x.Prize());

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
                from bonus in card.TotalPrizeCards 
                select new KeyValuePair<int, int>(
                    bonus,
                    counts[bonus] + counts[card.CardNumber])
                )
           ).Values.Sum();

    IEnumerable<ScratchCard> GetCards(string input)
        => from line in input.AsLines()
           let cardMatch = Regex.Match(line, @"Card\s+(\d+):")
           let split = line.Substring(cardMatch.Value.Length).Split('|')
           let winningNumbers = Regex.Matches(split[0], @"\d+").Select(m => int.Parse(m.Value)).ToArray()
           let numbers = Regex.Matches(split[1], @"\d+").Select(m => int.Parse(m.Value)).ToArray()
           select new ScratchCard(
               int.Parse(cardMatch.Result("$1")),
               winningNumbers,
               numbers
            );

    record ScratchCard(int CardNumber, int[] WinningNumbers, int[] Numbers)
    {
        public IEnumerable<int> MatchedNumbers =>
            Numbers.Where(WinningNumbers.Contains);

        public int Prize()
            => MatchedNumbers.Any()
            ? (int)Math.Pow(2, MatchedNumbers.Count() - 1)
            : 0;

        public IEnumerable<int> TotalPrizeCards
            => Enumerable.Range(CardNumber + 1, MatchedNumbers.Count());
    }
}
