using ExplodingKittens.Core.CardAggregate;
using ExplodingKittens.Core.GameAggregate.Contracts;

namespace ExplodingKittens.Core.ActionAggregate.Contracts;

/// <summary>
/// Factory to create played actions
/// </summary>
public interface IActionFactory
{
    /// <summary>
    /// Creates an action based on the provided parameters.
    /// </summary>
    /// <param name="game">Reference to the game which can be used to execute the action (if not noped).</param>
    /// <param name="playerId">Unique identifier of the player initiating the action.</param>
    /// <param name="cards">One or more cards involved in the action.</param>
    /// <param name="targetPlayerId">Optional unique identifier of the target player. Should be provided for actions like 'Favor' or 'StealRandomCard'.</param>
    /// <param name="targetCard">Optional target card involved in the action. Should be provided for actions like 'StealSpecificCard'.</param>
    /// <param name="drawPileIndex">
    /// Optional. Represents the position in the draw pile to insert a card.
    /// Is, for example, used in a 'Defuse' that allows a player to place an Exploding Kitten back into the draw pile at a specific position.</param>
    /// <returns>The created action.</returns>
    IAction Create(IGame game, Guid playerId, IReadOnlyList<Card> cards, Guid? targetPlayerId, Card? targetCard, int? drawPileIndex = null);
}