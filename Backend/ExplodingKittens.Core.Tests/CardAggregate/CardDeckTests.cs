
using ExplodingKittens.Core.CardAggregate;
using ExplodingKittens.Core.CardAggregate.Contracts;
using ExplodingKittens.Core.Tests.Extensions;
using Guts.Client.NUnit;

namespace ExplodingKittens.Core.Tests.CardAggregate;

[ProjectComponentTestFixture("1TINProject", "ExplodingKittens", "CardDeck",
    @"ExplodingKittens.Core\CardAggregate\CardDeck.cs;ExplodingKittens.Core\CardAggregate\Card.cs")]
public class CardDeckTests
{
    [MonitoredTest]
    public void Class_ShouldBeInternal_SoThatItCanOnlyBeUsedInTheCoreProject()
    {
        Assert.That(typeof(CardDeck).IsNotPublic, Is.True, "use 'internal class' instead of 'public class'");
    }

    [MonitoredTest]
    public void Class_ShouldImplement_ICardDeck()
    {
        Assert.That(typeof(CardDeck).IsAssignableTo(typeof(ICardDeck)), Is.True);
    }

    [MonitoredTest]
    public void ICardDeck_Interface_ShouldHaveCorrectMembers()
    {
        var type = typeof(ICardDeck);

        type.AssertInterfaceProperty(nameof(ICardDeck.CardCount), true, false);

        type.AssertInterfaceMethod(nameof(ICardDeck.InsertCard), typeof(void), typeof(Card), typeof(int));
        type.AssertInterfaceMethod(nameof(ICardDeck.Shuffle), typeof(void));
        type.AssertInterfaceMethod(nameof(ICardDeck.DrawTopCard), typeof(Card));
        type.AssertInterfaceMethod(nameof(ICardDeck.PeekTopCards), typeof(IReadOnlyList<Card>), typeof(int));
    }
}