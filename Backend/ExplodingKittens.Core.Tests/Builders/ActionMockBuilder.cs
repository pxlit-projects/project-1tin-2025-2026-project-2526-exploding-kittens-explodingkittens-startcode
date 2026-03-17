using ExplodingKittens.Core.ActionAggregate;
using ExplodingKittens.Core.ActionAggregate.Contracts;
using ExplodingKittens.Core.CardAggregate;
using ExplodingKittens.Core.Tests.Extensions;

namespace ExplodingKittens.Core.Tests.Builders;

public class ActionMockBuilder : MockBuilder<IAction>
{
    private readonly List<Card> _cards;

    public ActionMockBuilder()
    {
        _cards = [Random.Shared.NextCard()];
        Mock.SetupGet(a => a.PlayerId).Returns(Guid.NewGuid());
        Mock.SetupGet(a => a.Cards).Returns(() => _cards);
        Mock.SetupGet(a => a.CanBeNoped).Returns(true);
        Mock.SetupGet(a => a.TargetPlayerId).Returns(() => null);
        Mock.SetupProperty(a => a.TargetCard, null);
        Mock.SetupGet(a => a.DrawPileIndex).Returns(() => null);
        Mock.SetupGet(a => a.PlayerNopeDecisions).Returns(new Dictionary<Guid, NopeDecision>());
        Mock.SetupGet(a => a.IsExecuted).Returns(false);
    }

    public ActionMockBuilder WithCard(Card card)
    {
        _cards.Clear();
        _cards.Add(card);
        return this;
    }

    public ActionMockBuilder WithExecuted(bool executed)
    {
        Mock.SetupGet(a => a.IsExecuted).Returns(executed);
        return this;
    }
}