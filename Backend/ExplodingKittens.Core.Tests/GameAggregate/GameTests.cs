using ExplodingKittens.Core.ActionAggregate;
using ExplodingKittens.Core.ActionAggregate.Contracts;
using ExplodingKittens.Core.CardAggregate;
using ExplodingKittens.Core.GameAggregate;
using ExplodingKittens.Core.GameAggregate.Contracts;
using ExplodingKittens.Core.PlayerAggregate.Contracts;
using ExplodingKittens.Core.Tests.Builders;
using ExplodingKittens.Core.Tests.Extensions;
using Guts.Client.NUnit;
using Moq;

namespace ExplodingKittens.Core.Tests.GameAggregate;

[ProjectComponentTestFixture("1TINProject", "ExplodingKittens", "Game",
    @"ExplodingKittens.Core\GameAggregate\Game.cs")]
public class GameTests
{
    private IGame _game;
    private PlayerMockBuilder _player1MockBuilder;
    private PlayerMockBuilder _player2MockBuilder;
    private CardDeckMockBuilder _cardDeckMockBuilder;
    private ActionFactoryMockBuilder _actionFactoryMockBuilder;
    private IPlayer _player1;
    private IPlayer _player2;

    [SetUp]
    public void BeforeEachTest()
    {
        _actionFactoryMockBuilder = new ActionFactoryMockBuilder();
        _cardDeckMockBuilder = new CardDeckMockBuilder();
        _player1MockBuilder = new PlayerMockBuilder().WithName("PlayerA");
        _player1 = _player1MockBuilder.Object;
        _player2MockBuilder = new PlayerMockBuilder().WithName("PlayerB");
        _player2 = _player2MockBuilder.Object;
        IPlayer[] players = [_player1MockBuilder.Object, _player2MockBuilder.Object];
        _game = new Game(Guid.NewGuid(), players, _cardDeckMockBuilder.Object, _player1MockBuilder.Object.Id,
            _actionFactoryMockBuilder.Object) as IGame;
    }

    [MonitoredTest]
    public void Class_ShouldBeInternal_SoThatItCanOnlyBeUsedInTheCoreProject()
    {
        Assert.That(typeof(Game).IsNotPublic, Is.True, "use 'internal class' instead of 'public class'");
    }

    [MonitoredTest]
    public void Class_ShouldImplement_IGame()
    {
        Assert.That(typeof(Game).IsAssignableTo(typeof(IGame)), Is.True);
    }

    [MonitoredTest]
    public void IGame_Interface_ShouldHaveCorrectMembers()
    {
        var type = typeof(IGame);
        type.AssertInterfaceProperty(nameof(IGame.Id), true, false);
        type.AssertInterfaceProperty(nameof(IGame.Players), true, false);
        type.AssertInterfaceProperty(nameof(IGame.PlayerToPlayId), true, false);
        type.AssertInterfaceProperty(nameof(IGame.DiscardPile), true, false);
        type.AssertInterfaceProperty(nameof(IGame.DrawPile), true, false);
        type.AssertInterfaceProperty(nameof(IGame.PendingDraws), true, true);
        type.AssertInterfaceProperty(nameof(IGame.PendingAction), true, false);
        type.AssertInterfaceProperty(nameof(IGame.HasEnded), true, false);

        type.AssertInterfaceMethod(nameof(IGame.GetPlayerById), typeof(IPlayer), typeof(Guid));
        type.AssertInterfaceMethod(nameof(IGame.DrawCard), typeof(void), typeof(Guid));
        type.AssertInterfaceMethod(nameof(IGame.PlayAction), typeof(void), typeof(Guid), typeof(IReadOnlyList<Card>),
            typeof(Guid?), typeof(Card?), typeof(int?));
        type.AssertInterfaceMethod(nameof(IGame.NopePendingAction), typeof(void), typeof(Guid));
        type.AssertInterfaceMethod(nameof(IGame.ConfirmNotNopingPendingAction), typeof(void), typeof(Guid));
        type.AssertInterfaceMethod(nameof(IGame.SelectCardToGiveAsAFavor), typeof(void), typeof(Guid), typeof(Card));
        type.AssertInterfaceMethod(nameof(IGame.AdvanceTurn), typeof(void));
    }

    [MonitoredTest]
    public void DrawCard_NotPlayersTurn_ShouldThrowInvalidOperationException()
    {
        Class_ShouldImplement_IGame();

        // Act + Assert
        Assert.That(() => _game.DrawCard(_player2.Id),
            Throws.InvalidOperationException.With.Message.ContainsOne("turn", "beurt"));
    }

    [MonitoredTest]
    public void DrawCard_UncompletedPendingActionPresent_ShouldThrowInvalidOperationException()
    {
        Class_ShouldImplement_IGame();

        // Arrange
        _actionFactoryMockBuilder.WithCreateReturnsUnexecutedAction(Card.Favor);
        IReadOnlyList<Card> cards = [Card.Favor];
        _game.PlayAction(_player1.Id, cards, _player2.Id, null, null);
        _actionFactoryMockBuilder.Mock.Verify(a => a.Create(_game, _player1.Id, cards, _player2.Id, null, null),
            Times.Once, "The action factory should be used correctly to create the pending action.");

        //Act + Assert
        Assert.That(() => _game.DrawCard(_player1.Id),
            Throws.InvalidOperationException.With.Message.ContainsOne("actie", "action"));
    }

    [MonitoredTest]
    public void DrawCard_ShouldDrawCardFromDrawPileAndAddItToPlayersHand_ShouldAdvanceTurn()
    {
        Class_ShouldImplement_IGame();
        Assert.That(_game.PendingDraws, Is.EqualTo(1), "PendingDraws should be 1 at the start of the game.");

        // Arrange
        Card cardInDrawPile = Card.Skip;
        _cardDeckMockBuilder.WithDrawTopCardReturns(cardInDrawPile);

        // Act
        _game.DrawCard(_player1.Id);

        // Assert
        _cardDeckMockBuilder.Mock.Verify(d => d.DrawTopCard(), Times.Once,
            "A card should be drawn from the draw pile.");
        _player1MockBuilder.Mock.Verify(p => p.Hand.InsertCard(cardInDrawPile), Times.Once,
            "The drawn card should be added to the player's hand.");
        Assert.That(_game.PlayerToPlayId, Is.EqualTo(_player2.Id),
            "The turn should advance to the next player after drawing a card.");
        Assert.That(_game.PendingDraws, Is.EqualTo(1), "PendingDraws should be reset to 1 after advancing the turn to the other player.");
    }

    [MonitoredTest]
    public void PlayAction_UncompletedPendingActionPresent_ShouldThrowInvalidOperationException()
    {
        Class_ShouldImplement_IGame();

        // Arrange
        _actionFactoryMockBuilder.WithCreateReturnsUnexecutedAction(Card.SeeTheFuture);
        IReadOnlyList<Card> action1Cards = [Card.SeeTheFuture];
        _game.PlayAction(_player1.Id, action1Cards, null, null, null);

        List<Card> action2Cards = [Card.Cattermelon, Card.Cattermelon];


        //Act + Assert
        Assert.That(() => _game.PlayAction(_player1.Id, action2Cards, null, null, null),
            Throws.InvalidOperationException.With.Message.ContainsOne("actie", "action"));
    }

    [MonitoredTest]
    public void PlayAction_CompletedPendingActionPresent_ShouldThrowInvalidOperationException()
    {
        //Arrange

        _actionFactoryMockBuilder.WithCreateReturnsExecutedAction(Card.SeeTheFuture);
        IReadOnlyList<Card> action1Cards = [Card.SeeTheFuture];
        _game.PlayAction(_player1.Id, action1Cards, null, null, null);

        List<Card> action2Cards = [Card.HairyPotatoCat, Card.HairyPotatoCat];

        //Act
        _game.PlayAction(_player1.Id, action2Cards, _player2.Id, null, null);

        //Assert
        _actionFactoryMockBuilder.Mock.Verify(
            f => f.Create(_game, _player1.Id, action2Cards, _player2.Id, null, null), Times.Once,
            "The action factory is not used properly");
        int expectedDiscardedCardsCount = action1Cards.Count + action2Cards.Count;
        Assert.That(_game.DiscardPile, Has.Count.EqualTo(expectedDiscardedCardsCount),
            $"There should be {expectedDiscardedCardsCount} cards in the discard pile");
    }

    [MonitoredTest]
    public void PlayAction_ShouldUseFactoryToCreatePendingAction_ShouldRemoveCardsFromHand_ShouldAddCardsToDiscardPile()
    {
        //Arrange
        List<Card> cards = [Card.BeardCat, Card.BeardCat, Card.BeardCat];
        Guid? targetPlayerId = _player2.Id;
        Card? targetCard = Card.Defuse;
        int? drawPileIndex = null;

        //Act
        _game.PlayAction(_player1.Id, cards, targetPlayerId, targetCard, drawPileIndex);

        //Assert
        _actionFactoryMockBuilder.Mock.Verify(
            f => f.Create(_game, _player1.Id, cards, targetPlayerId, targetCard, drawPileIndex), Times.Once,
            "The action factory is not used properly");
        Assert.That(_game.DiscardPile, Has.Count.EqualTo(cards.Count),
            $"There should be {cards.Count} cards in the discard pile");
        Assert.That(_game.DiscardPile, Has.All.EqualTo(Card.BeardCat),
            "There should be 3 beard cats in the discard pile");

        _player1MockBuilder.Mock.Verify(p => p.Hand.PickSpecificCard(Card.BeardCat), Times.Exactly(cards.Count),
            "Each card played in an action should be removed from the players hand");
    }

    [MonitoredTest]
    public void NopePendingAction_NoPendingAction_ShouldThrowInvalidOperationException()
    {
        Class_ShouldImplement_IGame();

        // Act + Assert
        Assert.That(() => _game.NopePendingAction(_player2.Id),
            Throws.InvalidOperationException.With.Message.ContainsOne("action", "actie"));
    }

    [MonitoredTest]
    public void NopePendingAction_PlayerHasNoNopeCard_ShouldThrowInvalidOperationException()
    {
        Class_ShouldImplement_IGame();

        //Arrange
        _actionFactoryMockBuilder.WithCreateReturnsUnexecutedAction(Card.Attack);
        IReadOnlyList<Card> cards = [Card.Attack];
        _game.PlayAction(_player1.Id, cards, null, null, null);

        // Act + Assert
        Assert.That(() => _game.NopePendingAction(_player2.Id),
            Throws.InvalidOperationException.With.Message.ContainsOne("card", "kaart"));
    }

    [MonitoredTest]
    public void NopePendingAction_WithPendingAction_ShouldCallNopeOnAction_ShouldRemoveNopeCardFromHand_ShouldAddNopeCardToDiscardPile()
    {
        Class_ShouldImplement_IGame();

        // Arrange
        _player2MockBuilder.HandMockBuilder.WithCard(Card.Nope);
        _actionFactoryMockBuilder.WithCreateReturnsUnexecutedAction(Card.Attack);
        IReadOnlyList<Card> cards = [Card.Attack];
        _game.PlayAction(_player1.Id, cards, null, null, null);

        // Act
        _game.NopePendingAction(_player2.Id);

        // Assert
        _actionFactoryMockBuilder.ActionMock.Verify(a => a.Nope(_player2.Id), Times.Once, "The pending action is not noped correctly.");
        _player2MockBuilder.Mock.Verify(p => p.Hand.PickSpecificCard(Card.Nope), Times.Once,
            "A 'Nope' card should be removed from the player's hand");
    }

    [MonitoredTest]
    public void ConfirmNotNopingPendingAction_NoPendingAction_ShouldThrowInvalidOperationException()
    {
        Class_ShouldImplement_IGame();

        // Act + Assert
        Assert.That(() => _game.ConfirmNotNopingPendingAction(_player2.Id),
            Throws.InvalidOperationException.With.Message.ContainsOne("action", "actie"));
    }

    [MonitoredTest]
    public void ConfirmNotNopingPendingAction_WithPendingAction_ShouldCallConfirmNotNopingOnAction()
    {
        Class_ShouldImplement_IGame();

        // Arrange
        _actionFactoryMockBuilder.WithCreateReturnsUnexecutedAction(Card.Shuffle);
        IReadOnlyList<Card> cards = [Card.Shuffle];
        _game.PlayAction(_player1.Id, cards, null, null, null);

        // Act
        _game.ConfirmNotNopingPendingAction(_player2.Id);

        // Assert
        _actionFactoryMockBuilder.ActionMock.Verify(a => a.ConfirmNotNoping(_player2.Id), Times.Once,
            "Not noping is not confirmed correctly on the pending action");
    }

    [MonitoredTest]
    public void SelectCardToGiveAsAFavor_NoPendingAction_ShouldThrowInvalidOperationException()
    {
        Class_ShouldImplement_IGame();

        // Act + Assert
        Assert.That(() => _game.SelectCardToGiveAsAFavor(_player2.Id, Card.Skip),
            Throws.InvalidOperationException.With.Message.ContainsOne("action", "actie"));
    }

    [MonitoredTest]
    public void SelectCardToGiveAsAFavor_PendingActionIsNotFavor_ShouldThrowInvalidOperationException()
    {
        Class_ShouldImplement_IGame();

        // Arrange
        _actionFactoryMockBuilder.WithCreateReturnsUnexecutedAction(Card.SeeTheFuture);
        IReadOnlyList<Card> cards = [Card.SeeTheFuture];
        _game.PlayAction(_player1.Id, cards, null, null, null);

        // Act + Assert
        Assert.That(() => _game.SelectCardToGiveAsAFavor(_player2.Id, Card.RainbowRalphingCat),
            Throws.InvalidOperationException.With.Message.ContainsOne("cadeau", "favor", "geschenk"));
    }

    [MonitoredTest]
    public void SelectCardToGiveAsAFavor_ShouldSetTargetCardOfPendingAction()
    {
        Class_ShouldImplement_IGame();

        // Arrange
        IAction favorAction = new FavorAction(_game, _player1.Id, _player2.Id) as IAction;
        _actionFactoryMockBuilder.WithCreateReturnsUnexecutedAction(favorAction);
        IReadOnlyList<Card> cards = [Card.Favor];
        _game.PlayAction(_player1.Id, cards, _player2.Id, null, null);

        // Act
        _game.SelectCardToGiveAsAFavor(_player2.Id, Card.RainbowRalphingCat);

        // Assert
        Assert.That(_game.PendingAction, Is.Not.Null, "There should be a pending action.");
        Assert.That(_game.PendingAction.TargetCard, Is.EqualTo(Card.RainbowRalphingCat),
            "The target card of the pending action should be set to the selected card to give as a favor.");
    }

    [MonitoredTest]
    public void AdvanceTurn_WithPendingDraw_ShouldThrowInvalidOperationException()
    {
        Class_ShouldImplement_IGame();

        // Arrange
        _game.PendingDraws = 1;

        // Act + Assert
        Assert.That(() => _game.AdvanceTurn(),
            Throws.InvalidOperationException.With.Message.ContainsOne("draw", "trekken"));
    }

    [MonitoredTest]
    public void AdvanceTurn_ShouldClearFutureCardsForAllPlayers()
    {
        Class_ShouldImplement_IGame();

        // Arrange
        _game.PendingDraws = 0;
        List<Card> player1FutureCards = [Card.ExplodingKitten, Card.Attack, Card.BeardCat];
        _player1MockBuilder.WithFutureCards(player1FutureCards);
        List<Card> player2FutureCards = [Random.Shared.NextCard(), Random.Shared.NextCard(), Random.Shared.NextCard()];
        _player2MockBuilder.WithFutureCards(player2FutureCards);

        // Act
        _game.AdvanceTurn();

        // Assert
        _player1MockBuilder.Mock.VerifySet(p => p.FutureCards = It.Is<List<Card>>(list => list.Count == 0), Times.Once,
            "FutureCards should be cleared for player 1.");
        _player2MockBuilder.Mock.VerifySet(p => p.FutureCards = It.Is<List<Card>>(list => list.Count == 0), Times.Once,
            "FutureCards should be cleared for player 2.");
    }

    [MonitoredTest]
    public void AdvanceTurn_ShouldAdvanceToNextNonEliminatedPlayer()
    {
        Class_ShouldImplement_IGame();

        // Arrange
        Mock<IPlayer> player3Mock = new PlayerMockBuilder().WithName("PlayerC").Mock;
        IPlayer player3 = player3Mock.Object;
        IPlayer[] players = [_player1MockBuilder.Object, _player2MockBuilder.Object, player3];
        _game = new Game(Guid.NewGuid(), players, _cardDeckMockBuilder.Object, _player1MockBuilder.Object.Id,
            _actionFactoryMockBuilder.Object) as IGame;


        _game.PendingDraws = 0;
        _player2MockBuilder.WithEliminated(true);

        // Act
        _game.AdvanceTurn();

        // Assert
        Assert.That(_game.PlayerToPlayId, Is.EqualTo(player3.Id),
            "Turn should advance to the next non-eliminated player.");
        Assert.That(_game.PendingDraws, Is.EqualTo(1), "PendingDraws should be reset to 1.");
    }

    [MonitoredTest]
    public void HasEnded_MultiplePlayersNotEliminated_ShouldReturnFalse()
    {
        Class_ShouldImplement_IGame();

        // Arrange
        _player1MockBuilder.WithEliminated(false);
        _player2MockBuilder.WithEliminated(false);

        // Act + Assert
        Assert.That(_game.HasEnded, Is.False, "Game should not have ended when multiple players are not eliminated.");
    }

    [MonitoredTest]
    public void HasEnded_OnlyOnePlayerNotEliminated_ShouldReturnTrue()
    {
        Class_ShouldImplement_IGame();

        // Arrange
        var player1IsEliminated = Random.Shared.NextBool();
        _player1MockBuilder.WithEliminated(player1IsEliminated);
        _player2MockBuilder.WithEliminated(!player1IsEliminated);

        // Act + Assert
        Assert.That(_game.HasEnded, Is.True, "Game should have ended when only one player is not eliminated.");
    }
}