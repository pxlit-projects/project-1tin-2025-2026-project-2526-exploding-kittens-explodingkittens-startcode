using ExplodingKittens.Core.CardAggregate;

namespace ExplodingKittens.Api.Models.Output;

public class GameModel
{
    public Guid Id { get; set; }
    public List<PlayerInGameModel> Players { get; set; } = [];
    public List<Card> DiscardPile { get; set; } = [];
    public int DrawPileCount { get; set; }
    public Guid PlayerToPlayId { get; set; }
    public int PendingDraws { get; set; }
    public ActionModel? PendingAction { get; set; }     
    public bool HasEnded { get; set; }
}
