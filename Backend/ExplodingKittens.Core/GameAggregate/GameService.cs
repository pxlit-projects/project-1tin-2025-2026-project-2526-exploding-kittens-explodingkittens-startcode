using ExplodingKittens.Core.CardAggregate;
using ExplodingKittens.Core.GameAggregate.Contracts;

namespace ExplodingKittens.Core.GameAggregate;

/// <inheritdoc cref="IGameService"/>
internal class GameService : IGameService
{
    public GameService(IGameRepository gameRepository)
    {
    }

    public IGame GetGame(Guid gameId)
    {
        throw new NotImplementedException();
    }

    public IGame PlayAction(Guid gameId, Guid playerId, IReadOnlyList<Card> cards, Guid? targetPlayerId, Card? targetCard, int? drawPileIndex)
    {
        throw new NotImplementedException();

        //TODO (EXTRA): computer players should automatically decide whether to nope the action or not.
        //TODO (EXTRA): Also, if the action is a 'Favor' action, the computer player receiving the favor should automatically decide which card to give as a favor
    }

    public IGame DrawCard(Guid gameId, Guid playerId)
    {
        throw new NotImplementedException();

        //TODO (EXTRA): if the player at turn is a computer player, the computer player should decide its action automatically
    }

    public IGame NopePendingAction(Guid gameId, Guid playerId)
    {
        throw new NotImplementedException();

        //TODO (EXTRA): computer players should automatically decide whether to counter the nope or not
    }

    public IGame ConfirmNotNopingPendingAction(Guid gameId, Guid playerId)
    {
        throw new NotImplementedException();
    }

    public IGame SelectCardToGiveAsAFavor(Guid gameId, Guid playerId, Card card)
    {
        throw new NotImplementedException();
    }
}