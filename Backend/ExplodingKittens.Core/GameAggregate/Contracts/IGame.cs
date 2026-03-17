using ExplodingKittens.Core.ActionAggregate.Contracts;
using ExplodingKittens.Core.CardAggregate;
using ExplodingKittens.Core.CardAggregate.Contracts;
using ExplodingKittens.Core.PlayerAggregate.Contracts;

namespace ExplodingKittens.Core.GameAggregate.Contracts;

/// <summary>
/// Represents an exploding kittens game in progress, including the players, their hands, the draw pile, and the discard pile.
/// </summary>
public interface IGame
{
    /// <summary>
    /// The unique identifier of the game
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// The players (minimum 2, maximum 5) that are playing the game
    /// </summary>
    IPlayer[] Players { get; }

    /// <summary>
    /// The unique identifier of the player whose turn it is
    /// </summary>
    Guid PlayerToPlayId { get; }

    /// <summary>
    /// List of cards that have been discarded during the game.
    /// The last card in the list is the most recently discarded card.
    /// </summary>
    IList<Card> DiscardPile { get; }

    /// <summary>
    /// The deck of cards that players draw from during the game.
    /// </summary>
    ICardDeck DrawPile { get; }

    /// <summary>
    /// Number of cards the current player must draw before their turn ends (0 = normal draw 1; 2 = Attack).
    /// </summary>
    int PendingDraws { get; set; }

    /// <summary>
    /// The action that is currently pending for execution.
    /// </summary>
    /// <remarks>If no action is pending, this property returns null</remarks>
    IAction? PendingAction { get; }

    /// <summary>
    /// Indicates whether the game has ended (one player left or all but one eliminated).
    /// </summary>
    bool HasEnded { get; }

    /// <summary>
    /// Get a player by their unique identifier.
    /// </summary>
    /// <param name="playerId">The unique identifier of the player</param>
    /// <returns>The player with the specified unique identifier</returns>
    IPlayer GetPlayerById(Guid playerId);

    /// <summary>
    /// Draws a card for the specified player and adds it to their hand.
    /// </summary>
    /// <param name="playerId">The unique identifier of the player for whom the card is drawn.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if
    /// - It's not the specified player's turn
    /// - There is a pending action that has not been executed
    /// </exception>
    void DrawCard(Guid playerId);

    /// <summary>
    /// Executes a player action using the specified cards, optionally targeting another player or card, and optionally
    /// specifying a position in the draw pile.
    /// </summary>
    /// <param name="playerId">The unique identifier of the player performing the action.</param>
    /// <param name="cards">One or more cards used to perform the action.</param>
    /// <param name="targetPlayerId">
    /// An optional unique identifier of the player to target with the action.
    /// If null, no specific player is targeted.
    /// </param>
    /// <param name="targetCard">An optional card to target with the action. If null, no specific card is targeted.</param>
    /// <param name="drawPileIndex">
    /// An optional index indicating the position in the draw pile where the <see cref="targetCard"/> can be inserted.
    /// If null, no card will be inserted into the draw pile.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if
    /// - It's not the specified player's turn
    /// - There already is a pending action that has not been executed
    /// </exception>
    void PlayAction(Guid playerId, IReadOnlyList<Card> cards, Guid? targetPlayerId, Card? targetCard, int? drawPileIndex);

    /// <summary>
    /// Sets the nope decision for the specified player on the pending action to "nope", possibly canceling the pending action.
    /// </summary>
    /// <param name="playerId">The unique identifier of the player making the nope decision.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if
    /// - There is no pending action
    /// - The specified player has no 'Nope' card in their hand
    /// </exception>
    void NopePendingAction(Guid playerId);

    /// <summary>
    /// Sets the nope decision for the specified player on the pending action to "not nope", confirming that the pending action can be executed.
    /// </summary>
    /// <param name="playerId">The unique identifier of the player making the decision not to nope.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if
    /// - There is no pending action
    /// </exception>
    void ConfirmNotNopingPendingAction(Guid playerId);

    /// <summary>
    /// Sets the target card of a pending 'Favor' action to the specified card, indicating that the specified player has selected that card to be given as a favor.
    /// This normally also triggers the execution of the pending 'Favor' action.
    /// </summary>
    /// <param name="playerId">The unique identifier of the player that is giving a card as a favor.</param>
    /// <param name="card">The card to be given.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if
    /// - There is no pending action
    /// </exception>
    void SelectCardToGiveAsAFavor(Guid playerId, Card card);

    /// <summary>
    /// Advance to the next active player's turn. Skips eliminated players.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if
    /// - The current player has pending draws that have not been drawn yet
    /// </exception>
    void AdvanceTurn();
}