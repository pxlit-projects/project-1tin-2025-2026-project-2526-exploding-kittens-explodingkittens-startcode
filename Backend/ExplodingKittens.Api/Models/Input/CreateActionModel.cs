using ExplodingKittens.Core.CardAggregate;

namespace ExplodingKittens.Api.Models.Input;

public class CreateActionModel
{
    public List<Card> Cards { get; set; } = new List<Card>();
    public Guid? TargetPlayerId { get; set; }
    public Card? TargetCard { get; set; }
    public int? DrawPileIndex { get; set; }
}