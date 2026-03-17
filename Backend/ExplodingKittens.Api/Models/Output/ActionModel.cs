using ExplodingKittens.Core.ActionAggregate;
using ExplodingKittens.Core.CardAggregate;

namespace ExplodingKittens.Api.Models.Output;

public class ActionModel
{
    public Guid PlayerId { get; set; }
    public List<Card> Cards { get; set; } = [];
    public bool CanBeNoped { get; set; }
    public Guid? TargetPlayerId { get; set; }
    public Card? TargetCard { get; set; }
    public int? DrawPileIndex { get; set; }
    public IReadOnlyDictionary<Guid, NopeDecision> PlayerNopeDecisions { get; set; } =
        new Dictionary<Guid, NopeDecision>();
    public bool IsExecuted { get; set; }
}