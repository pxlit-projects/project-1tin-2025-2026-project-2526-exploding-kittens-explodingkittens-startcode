namespace ExplodingKittens.Core.CardAggregate.Contracts;

/// <summary>
/// Defines a factory interface for creating card decks
/// </summary>
public interface ICardDeckFactory
{
    /// <summary>
    /// Creates a deck of cards for the standard game.
    /// The created deck should be used to give each player their initial hand. So the deck does not contain exploding kittens (yet). 
    /// </summary>
    /// <param name="numberOfPlayers">The number of players participating in the game.</param>
    /// <returns>A standard deck without exploding kittens.</returns>
    ICardDeck CreateStandardDeckWithoutExplodingKittens(int numberOfPlayers);
}