using ExplodingKittens.Core.GameAggregate.Contracts;
using ExplodingKittens.Core.PlayerAggregate.Contracts;

namespace ExplodingKittens.Core.PlayerAggregate;

/// <inheritdoc cref="IGamePlayStrategy"/>
internal class GamePlayStrategy : IGamePlayStrategy
{
    public void ReactToPendingActionOfOtherPlayer(Guid playerId, IGame game)
    {
        throw new NotImplementedException("EXTRA: play against computer");
    }

    public void PlayActionOrDrawCard(Guid playerId, IGame game)
    {
        throw new NotImplementedException("EXTRA: play against computer");
    }

    public void ReactToThePendingActionBeingNoped(Guid playerId, IGame game)
    {
        throw new NotImplementedException("EXTRA: play against computer");
    }
}