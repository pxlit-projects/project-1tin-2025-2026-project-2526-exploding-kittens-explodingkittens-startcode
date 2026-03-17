using ExplodingKittens.Core.CardAggregate;
using ExplodingKittens.Core.GameAggregate.Contracts;

namespace ExplodingKittens.Core.ActionAggregate;

internal class StealSpecificCardAction
{
    public StealSpecificCardAction(IGame game, Guid playerId, IReadOnlyList<Card> cards, Card targetCard, Guid targetPlayerId)
    {
    }
}