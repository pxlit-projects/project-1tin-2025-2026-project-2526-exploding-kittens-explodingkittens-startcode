using ExplodingKittens.Core.GameAggregate.Contracts;

namespace ExplodingKittens.Core.ActionAggregate;

/// <summary>End your turn without drawing a card.</summary>
internal class SkipAction
{
    public SkipAction(IGame game, Guid playerId)
    {
    }
}