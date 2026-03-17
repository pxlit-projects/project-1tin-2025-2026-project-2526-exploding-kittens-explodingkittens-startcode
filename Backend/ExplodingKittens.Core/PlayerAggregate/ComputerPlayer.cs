using ExplodingKittens.Core.GameAggregate.Contracts;
using ExplodingKittens.Core.PlayerAggregate.Contracts;

namespace ExplodingKittens.Core.PlayerAggregate;

/// <inheritdoc cref="IPlayer"/>
internal class ComputerPlayer : PlayerBase
{
    private readonly IGamePlayStrategy _strategy;

    public ComputerPlayer(string name, IGamePlayStrategy strategy) : base(Guid.NewGuid(), name, DateOnly.MinValue)
    {
        _strategy = strategy;
    }

    public void ReactToPendingActionOfOtherPlayer(IGame game)
    {
        throw new NotImplementedException("EXTRA: play against computer");
    }

    public void PlayActionOrDrawCard(IGame game)
    {
        throw new NotImplementedException("EXTRA: play against computer");
    }

    public void ReactToThePendingActionBeingNoped(IGame game)
    {
        throw new NotImplementedException("EXTRA: play against computer");
    }
}