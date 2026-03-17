using ExplodingKittens.Core.CardAggregate;

namespace ExplodingKittens.Core.GameAggregate.Contracts;

public interface IGameService
{
    /// <summary>
    /// Retrieves a game from the repository of games.
    /// </summary>
    IGame GetGame(Guid gameId);

    /// <summary>
    /// Retrieves a game from the repository of games and performs the specified action on it.
    /// </summary>
    /// <returns>The updated game</returns>
    IGame PlayAction(Guid gameId, Guid playerId, IReadOnlyList<Card> cards, Guid? targetPlayerId, Card? targetCard, int? drawPileIndex);

    /// <summary>
    /// Retrieves a game from the repository of games and draws a card for the specified player.
    /// </summary>
    /// <returns>The updated game</returns>
    IGame DrawCard(Guid gameId, Guid playerId);

    /// <summary>
    /// Retrieves a game from the repository of games and sets a nope decision on the pending action.
    /// </summary>
    /// <returns>The updated game</returns>
    IGame NopePendingAction(Guid gameId, Guid playerId);

    /// <summary>
    /// Retrieves a game from the repository of games and sets a not-noping decision on the pending action.
    /// </summary>
    /// <returns>The updated game</returns>
    IGame ConfirmNotNopingPendingAction(Guid gameId, Guid playerId);

    /// <summary>
    /// Retrieves a game from the repository of games and sets the target card of the pending favor action.
    /// </summary>
    /// <returns>The updated game</returns>
    IGame SelectCardToGiveAsAFavor(Guid gameId, Guid playerId, Card card);
}