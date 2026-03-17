using ExplodingKittens.Core.CardAggregate;
using ExplodingKittens.Core.GameAggregate;
using ExplodingKittens.Core.GameAggregate.Contracts;
using ExplodingKittens.Core.Tests.Extensions;
using Guts.Client.NUnit;
using Moq;

namespace ExplodingKittens.Core.Tests.GameAggregate;

[ProjectComponentTestFixture("1TINProject", "ExplodingKittens", "GameService",
    @"ExplodingKittens.Core\GameAggregate\GameService.cs")]
public class GameServiceTests
{
    private Mock<IGameRepository> _gameRepositoryMock = null!;
    private GameService _gameService = null!;
    private Guid _gameId;
    private Guid _playerId;

    [SetUp]
    public void SetUp()
    {
        _gameRepositoryMock = new Mock<IGameRepository>();
        _gameService = new GameService(_gameRepositoryMock.Object);
        _gameId = Guid.NewGuid();
        _playerId = Guid.NewGuid();
    }

    [MonitoredTest]
    public void GetGame_ShouldUseRepositoryToRetrieveGame()
    {
        // Arrange
        var gameMock = new Mock<IGame>();
        _gameRepositoryMock.Setup(repo => repo.GetById(_gameId)).Returns(gameMock.Object);

        // Act
        IGame? result = _gameService.GetGame(_gameId);

        // Assert
        Assert.That(gameMock.Object, Is.SameAs(result));
    }

    [MonitoredTest]
    public void PlayAction_ShouldUseRepositoryToRetrieveGame_ShouldCallPlayActionOnGame()
    {
        // Arrange
        var gameMock = new Mock<IGame>();
        _gameRepositoryMock.Setup(repo => repo.GetById(_gameId)).Returns(gameMock.Object);
        List<Card> cards = [Random.Shared.NextCard()]; 
        Guid? targetPlayerId = Guid.NewGuid();
        Card? targetCard = Random.Shared.NextCard();
        int? drawPileIndex = Random.Shared.Next(0, 4);

        // Act
        _gameService.PlayAction(_gameId, _playerId, cards, targetPlayerId, targetCard, drawPileIndex);

        // Assert
        gameMock.Verify(game => game.PlayAction(_playerId, cards, targetPlayerId, targetCard, drawPileIndex), Times.Once);
    }

    [MonitoredTest]
    public void DrawCard_ShouldUseRepositoryToRetrieveGame_ShouldCallDrawCardOnGame()
    {
        // Arrange
        var gameMock = new Mock<IGame>();
        _gameRepositoryMock.Setup(repo => repo.GetById(_gameId)).Returns(gameMock.Object);

        // Act
        _gameService.DrawCard(_gameId, _playerId);

        // Assert
        gameMock.Verify(game => game.DrawCard(_playerId), Times.Once);
    }

    [MonitoredTest]
    public void NopePendingAction_ShouldUseRepositoryToRetrieveGame_ShouldCallNopePendingActionOnGame()
    {
        // Arrange
        var gameMock = new Mock<IGame>();
        _gameRepositoryMock.Setup(repo => repo.GetById(_gameId)).Returns(gameMock.Object);

        // Act
        _gameService.NopePendingAction(_gameId, _playerId);

        // Assert
        gameMock.Verify(game => game.NopePendingAction(_playerId), Times.Once);
    }

    [MonitoredTest]
    public void ConfirmNotNopingPendingAction_ShouldUseRepositoryToRetrieveGame_ShouldCallConfirmNotNopingPendingActionOnGame()
    {
        // Arrange
        var gameMock = new Mock<IGame>();
        _gameRepositoryMock.Setup(repo => repo.GetById(_gameId)).Returns(gameMock.Object);

        // Act
        _gameService.ConfirmNotNopingPendingAction(_gameId, _playerId);

        // Assert
        gameMock.Verify(game => game.ConfirmNotNopingPendingAction(_playerId), Times.Once);
    }
}