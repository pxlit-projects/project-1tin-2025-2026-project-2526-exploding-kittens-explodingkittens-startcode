using ExplodingKittens.Core.ActionAggregate.Contracts;
using ExplodingKittens.Core.CardAggregate;
using ExplodingKittens.Core.GameAggregate.Contracts;

namespace ExplodingKittens.Core.ActionAggregate;

/// <inheritdoc cref="IActionFactory"/>
internal class ActionFactory : IActionFactory
{
    public IAction Create(IGame game, Guid playerId, IReadOnlyList<Card> cards, Guid? targetPlayerId, Card? targetCard,
        int? drawPileIndex = null)
    {
        throw new NotImplementedException();
    }
}
