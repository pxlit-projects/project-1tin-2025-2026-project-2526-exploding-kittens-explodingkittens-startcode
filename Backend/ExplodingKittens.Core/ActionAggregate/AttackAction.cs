using ExplodingKittens.Core.GameAggregate.Contracts;

namespace ExplodingKittens.Core.ActionAggregate;

/// <summary>
/// Player can skip turn and the next player must draw twice as many cards as the current player did.
/// </summary>
internal class AttackAction
{
    public AttackAction(IGame game, Guid playerId)
    {
    }
}
