using ExplodingKittens.Core.ActionAggregate;
using ExplodingKittens.Core.ActionAggregate.Contracts;
using ExplodingKittens.Core.CardAggregate;
using ExplodingKittens.Core.GameAggregate.Contracts;
using ExplodingKittens.Core.PlayerAggregate.Contracts;
using ExplodingKittens.Core.Tests.Builders;
using ExplodingKittens.Core.Tests.Extensions;
using Guts.Client.NUnit;
using Moq;

namespace ExplodingKittens.Core.Tests.ActionAggregate;

[ProjectComponentTestFixture("1TINProject", "ExplodingKittens", "ActionFactory",
    @"ExplodingKittens.Core\ActionAggregate\ActionFactory.cs")]
public class ActionFactoryTests
{
    private IActionFactory _actionFactory;
    private Mock<IGame> _gameMock;

    [SetUp]
    public void SetUp()
    {
        _gameMock = new GameMockBuilder().Mock;
        _actionFactory = new ActionFactory() as IActionFactory;
    }

    [MonitoredTest]
    public void Class_ShouldBeInternal_SoThatItCanOnlyBeUsedInTheCoreProject()
    {
        Assert.That(typeof(ActionFactory).IsNotPublic, Is.True, "use 'internal class' instead of 'public class'");
    }

    [MonitoredTest]
    public void Class_ShouldImplement_IActionFactory()
    {
        Assert.That(typeof(ActionFactory).IsAssignableTo(typeof(IActionFactory)), Is.True);
    }

    [MonitoredTest]
    public void IActionFactory_Interface_ShouldHaveCorrectMembers()
    {
        var type = typeof(IActionFactory);
        type.AssertInterfaceMethod(nameof(IActionFactory.Create), typeof(IAction), typeof(IGame), typeof(Guid), typeof(IReadOnlyList<Card>), typeof(Guid?), typeof(Card?), typeof(int?));
    }

    [MonitoredTest]
    [TestCase(typeof(StealRandomCardAction), true, null, null, Card.BeardCat, Card.BeardCat)]
    [TestCase(typeof(StealSpecificCardAction), true, Card.Defuse, null, Card.Cattermelon, Card.Cattermelon, Card.Cattermelon)]
    [TestCase(typeof(DefuseAction), false, Card.ExplodingKitten, 2, Card.Defuse)]
    [TestCase(typeof(AttackAction), false, null, null, Card.Attack)]
    [TestCase(typeof(FavorAction), true, null, null, Card.Favor)]
    [TestCase(typeof(SkipAction), false, null, null, Card.Skip)]
    [TestCase(typeof(ShuffleAction), false, null, null, Card.Shuffle)]
    [TestCase(typeof(SeeTheFutureAction), false, null, null, Card.SeeTheFuture)]
    public void Create_ShouldReturnCorrectActionTypeDependingOnTheCards(Type expectedActionType, bool pickTargetPlayer, Card? targetCard, int? drawPileIndex, params Card[] cards)
    {
        //Arrange
        IGame game = _gameMock.Object;
        IPlayer player1 = game.Players.ElementAt(0);
        IPlayer player2 = game.Players.ElementAt(1);
        Guid? targetPlayerId = pickTargetPlayer ? player2.Id : null;

        //Act
        IAction result = _actionFactory.Create(game, player1.Id, cards, targetPlayerId, targetCard, drawPileIndex);

        //Assert
        Assert.That(result, Is.InstanceOf(expectedActionType), $"For the cards '{string.Join(",", cards)}', the returned action should be a '{expectedActionType.Name}'");
    }

    [MonitoredTest]
    [TestCase("At least one card must be provided", false, null, null)]
    [TestCase("Different cat cards are not allowed", true, null, null, Card.TacoCat, Card.HairyPotatoCat)]
    [TestCase("Different cat cards are not allowed", true, Card.SeeTheFuture, null, Card.BeardCat, Card.BeardCat, Card.HairyPotatoCat)]
    [TestCase("When multiple cat cards are played, a target player must be specified", false, null, null, Card.TacoCat, Card.TacoCat)]
    [TestCase("When three cat cards are played, a target card must be specified", true, null, null, Card.TacoCat, Card.TacoCat, Card.TacoCat)]
    [TestCase("An exploding kitten card cannot be used to create an action", false, null, 1, Card.ExplodingKitten)]
    [TestCase("To create a 'Defuse' action, the target card must be an exploding kitten", false, Card.SeeTheFuture, 0, Card.Defuse)]
    [TestCase("To create a 'Defuse' action, a draw pile index must be specified", false, Card.ExplodingKitten, null, Card.Defuse)]
    [TestCase("A target player must be specified for a 'Favor' action", false, null, null, Card.Favor)]
    [TestCase("A 'Nope' card cannot be used to create an action", false, null, null, Card.Nope)]
    [TestCase("Action cards and cat cards cannot be mixed", false, null, null, Card.Shuffle, Card.TacoCat)]
    public void Create_InvalidInput_ShouldThrowInvalidOperationException(string expectation, bool pickTargetPlayer, Card? targetCard, int? drawPileIndex, params Card[] cards)
    {
        //Arrange
        IGame game = _gameMock.Object;
        IPlayer player1 = game.Players.ElementAt(0);
        IPlayer player2 = game.Players.ElementAt(1);
        Guid? targetPlayerId = pickTargetPlayer ? player2.Id : null;

        //Act & Assert
        Assert.That(() => _actionFactory.Create(game, player1.Id, cards, targetPlayerId, targetCard, drawPileIndex), Throws.InvalidOperationException, expectation);
    }
}