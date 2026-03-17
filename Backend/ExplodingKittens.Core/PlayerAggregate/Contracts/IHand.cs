using ExplodingKittens.Core.CardAggregate;

namespace ExplodingKittens.Core.PlayerAggregate.Contracts;

/// <summary>
/// Represents a set of cards that a player is currently holding.
/// </summary>
public interface IHand
{
    /// <summary>
    /// The cards currently in the player's hand
    /// </summary>
    IReadOnlyList<Card> Cards { get; }

    /// <summary>
    /// Inserts a card into the hand.
    /// The cards in a hand are sorted in ascending order based on their enum values.
    /// When a new card is inserted, it is placed in the correct position to maintain this sorted order.
    /// </summary>
    /// <param name="card">The card to be inserted</param>
    void InsertCard(Card card);

    /// <summary>
    /// Determines whether the specified card is present in the collection.
    /// </summary>
    /// <param name="card">The card to locate in the hand.</param>
    /// <returns>true if the card is found in the hand; otherwise, false.</returns>
    bool Contains(Card card);

    /// <summary>
    /// Selects a specific card from the hand that matches the provided card.
    /// </summary>
    /// <param name="card">The card to locate and select.</param>
    /// <returns>Returns the matching card if found; otherwise, returns null.</returns>
    Card? PickSpecificCard(Card card);

    /// <summary>
    /// Selects a random card from the hand.
    /// </summary>
    /// <returns>Returns a random card if the hand is not empty; otherwise, returns null.</returns>
    Card? PickRandomCard();
}