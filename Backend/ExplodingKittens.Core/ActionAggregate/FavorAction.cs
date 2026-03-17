using ExplodingKittens.Core.GameAggregate.Contracts;

namespace ExplodingKittens.Core.ActionAggregate;

/// <summary>Target player gives you a card of their choice. Target chooses which card to give.</summary>
internal class FavorAction
{
    public FavorAction(IGame game, Guid playerId, Guid targetPlayerId)
    {
    }

}
