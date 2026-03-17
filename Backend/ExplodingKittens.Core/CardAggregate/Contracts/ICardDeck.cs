namespace ExplodingKittens.Core.CardAggregate.Contracts;

/// <summary>
/// Represents a deck of cards.
/// </summary>
public interface ICardDeck
{
    /// <summary>
    /// The total number of cards in the deck.
    /// </summary>
    int CardCount { get; }

    /// <summary>
    /// Inserts a card (e.g. an exploding kitten) into the deck at the specified position.
    /// </summary>
    /// <param name="card">The card to insert</param>
    /// <param name="positionFromTop">0 = top of the deck</param>
    void InsertCard(Card card, int positionFromTop = 0);

    /// <summary>
    /// Randomly rearranges the positions of the cards in the deck.
    /// </summary>
    void Shuffle();

    /// <summary>
    /// Removes the top card from the deck and returns it.
    /// </summary>
    /// <returns>The top card from the deck.</returns>
    Card DrawTopCard();

    /// <summary>
    /// Returns a read-only list of the top cards from the deck without removing them.
    /// </summary>
    /// <param name="numberOfCards">The number of top cards to peek at.</param>
    /// <returns>A read-only list of the top cards.</returns>
    IReadOnlyList<Card> PeekTopCards(int numberOfCards);
}