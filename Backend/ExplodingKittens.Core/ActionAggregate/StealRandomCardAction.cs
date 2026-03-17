using ExplodingKittens.Core.CardAggregate;
using ExplodingKittens.Core.GameAggregate.Contracts;

namespace ExplodingKittens.Core.ActionAggregate;

internal class StealRandomCardAction
{
    public StealRandomCardAction(IGame game, Guid playerId, IReadOnlyList<Card> cards, Guid targetPlayerId) 
    {
    }

}