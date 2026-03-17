using ExplodingKittens.Core.CardAggregate;

namespace ExplodingKittens.Core.PlayerAggregate.Contracts;

/// <summary>
/// Represents a player in the game.
/// </summary>
public interface IPlayer
{
    /// <summary>
    /// Unique identifier of the player
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// (Display) name of the player
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The date the player was born. Can be used to determine the youngest player of a game.
    /// </summary>
    public DateOnly BirthDate { get; }

    /// <summary>
    /// The cards this player is currently holding
    /// </summary>
    public IHand Hand { get; }

    /// <summary>
    /// Indicates whether the player has an exploding kitten in their hand.
    /// </summary>
    public bool HasExplodingKitten { get; }

    /// <summary>
    /// Indicates whether the player is eliminated from the game.
    /// A player is eliminated if they have an exploding kitten in their hand and do not have a defuse card to defuse it.
    /// </summary>
    public bool Eliminated { get; }

    /// <summary>
    /// The cards the player has seen after playing a "See the Future" card.
    /// This list is cleared at the end of the player's turn.
    /// </summary>
    public IReadOnlyList<Card> FutureCards { get; set; }
}