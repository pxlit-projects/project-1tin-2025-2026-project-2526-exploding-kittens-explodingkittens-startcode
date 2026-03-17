using ExplodingKittens.Core.ActionAggregate.Contracts;
using ExplodingKittens.Core.CardAggregate;
using ExplodingKittens.Core.GameAggregate.Contracts;

namespace ExplodingKittens.Core.ActionAggregate;

/// <inheritdoc cref="IAction"/>
public abstract class ActionBase
{
    protected ActionBase(IGame game, Guid playerId, IReadOnlyList<Card> cards, bool canBeNoped)
    {
        
    }

    protected ActionBase(IGame game, Guid playerId, IReadOnlyList<Card> cards, bool canBeNoped, Card? targetCard, Guid? targetPlayerId, int? drawPileIndex) : this(game, playerId, cards, canBeNoped)
    {
        
    }

    /// <summary>
    /// Classes that inherit from ActionBase should implement this method to execute the specific logic of the action.
    /// </summary>
    protected abstract void Execute();
}