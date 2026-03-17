using ExplodingKittens.Core.GameAggregate.Contracts;

namespace ExplodingKittens.Core.ActionAggregate;

/// <summary>
/// Defuse an Exploding Kitten: place it back in the draw pile at the chosen index.
/// The turn is over after playing this card
/// </summary>
internal class DefuseAction
{
    public DefuseAction(IGame game, Guid playerId, int drawPileIndex)
    {
        
    }
}
