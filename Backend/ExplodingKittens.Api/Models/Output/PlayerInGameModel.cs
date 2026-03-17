using ExplodingKittens.Core.CardAggregate;

namespace ExplodingKittens.Api.Models.Output;

/// <summary>
/// Player view in a game. For the requesting user, <see cref="CardsInHand"/> is populated;
/// for other players only <see cref="CardsInHandCount"/> is set and <see cref="CardsInHand"/> is null.
/// </summary>
public class PlayerInGameModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public bool HasExplodingKitten { get; set; }
    public bool Eliminated { get; set; }
    public List<Card> FutureCards { get; set; } = [];

    /// <summary>
    /// Number of cards in hand.
    /// </summary>
    public int CardsInHandCount { get; set; }

    /// <summary>
    /// Actual cards in hand. Only populated for the requesting user; empty for other players.
    /// </summary>
    public List<Card> CardsInHand { get; set; } = [];
}
