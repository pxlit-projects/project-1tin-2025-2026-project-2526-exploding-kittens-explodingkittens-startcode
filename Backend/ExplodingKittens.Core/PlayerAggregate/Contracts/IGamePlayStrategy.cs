using ExplodingKittens.Core.GameAggregate.Contracts;

namespace ExplodingKittens.Core.PlayerAggregate.Contracts;

/// <summary>
/// A strategy for an AI player. The strategy can be used to determine a move for the AI player in a game.
/// </summary>
/// <remarks>This is an EXTRA. Not needed to implement the minimal requirements.</remarks>
public interface IGamePlayStrategy
{
    /// <summary>
    /// Handles the pending actions of another player in the game, allowing the computer player to react according to the
    /// game's rules.
    /// </summary>
    /// <remarks>This method is typically invoked when another player has actions awaiting resolution. The
    /// implementation should determine the appropriate response based on the game's current state and rules.</remarks>
    /// <param name="playerId">The identifier of the computer player that needs to react on a pending action</param>
    /// <param name="game">The game instance that provides the current state and context</param>
    void ReactToPendingActionOfOtherPlayer(Guid playerId, IGame game);

    /// <summary>
    /// Executes an action in the game or draws a card based on the current game state.
    /// </summary>
    /// <param name="playerId">The identifier of the computer player</param>
    /// <param name="game">The game instance that provides the current state and context</param>
    void PlayActionOrDrawCard(Guid playerId, IGame game);

    /// <summary>
    /// Reacts to the pending action being noped by another player.
    /// The computer player can decide to nope the nope or to confirm to do nothing.
    /// </summary>
    /// <param name="playerId">The identifier of the computer player that needs to react on a nope of another player</param>
    /// <param name="game">The game instance that provides the current state and context</param>
    void ReactToThePendingActionBeingNoped(Guid playerId, IGame game);
}