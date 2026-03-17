using System.Security.Claims;
using ExplodingKittens.Api.Controllers;
using ExplodingKittens.Api.Models;
using ExplodingKittens.Api.Models.Input;
using ExplodingKittens.Api.Models.Output;
using ExplodingKittens.Core.CardAggregate;
using ExplodingKittens.Core.GameAggregate.Contracts;
using ExplodingKittens.Core.Tests.Builders;
using Guts.Client.NUnit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ExplodingKittens.Api.Tests;

[ProjectComponentTestFixture("1TINProject", "ExplodingKittens", "GamesController",
    @"ExplodingKittens.Api\Controllers\GamesController.cs;ExplodingKittens.Api\Models\ModelMapper.cs")]
public class GamesControllerTests
{
    private GamesController _controller = null!;
    private Mock<IGameService> _gameServiceMock = null!;
    private Mock<IModelMapper> _mapperMock = null!;
    private Guid _loggedInUserId;

    [SetUp]
    public void Setup()
    {
        _gameServiceMock = new Mock<IGameService>();
        _mapperMock = new Mock<IModelMapper>();

        _controller = new GamesController(_gameServiceMock.Object, _mapperMock.Object);

        _loggedInUserId = Guid.NewGuid();
        var userClaimsPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity(new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, _loggedInUserId.ToString())
            })
        );
        var context = new ControllerContext { HttpContext = new DefaultHttpContext() };
        context.HttpContext.User = userClaimsPrincipal;
        _controller.ControllerContext = context;
    }

    [MonitoredTest]
    public void GetById_ShouldUseGameServiceToGetGame_ShouldReturnMappedModel()
    {
        //Arrange
        IGame game = new GameMockBuilder().Object;
        _gameServiceMock.Setup(service => service.GetGame(game.Id)).Returns(game);

        var gameModel = new GameModel();
        _mapperMock.Setup(mapper => mapper.MapGame(game, _loggedInUserId)).Returns(gameModel);

        //Act
        var result = _controller.GetById(game.Id) as OkObjectResult;

        //Assert
        Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");

        _gameServiceMock.Verify(service => service.GetGame(game.Id), Times.Once,
            "The 'GetGame' method of the game service is not called correctly");

        _mapperMock.Verify(mapper => mapper.MapGame(game, _loggedInUserId), Times.Once,
            "The game is not correctly mapped to a game model");

        Assert.That(result!.Value, Is.SameAs(gameModel), "The mapped game model should be in the OkObjectResult");
    }

    [MonitoredTest]
    public void PlayAction_ShouldUseGameServiceToPlayAction_ShouldReturnMappedModel()
    {
        //Arrange
        Guid gameId = Guid.NewGuid();
        var input = new CreateActionModel
        {
            Cards = new List<Card> { Card.Attack },
            TargetPlayerId = null,
            TargetCard = null,
            DrawPileIndex = null
        };

        IGame game = new GameMockBuilder().Object;
        _gameServiceMock
            .Setup(service => service.PlayAction(gameId, _loggedInUserId, input.Cards, input.TargetPlayerId, input.TargetCard, input.DrawPileIndex))
            .Returns(game);

        var gameModel = new GameModel();
        _mapperMock.Setup(mapper => mapper.MapGame(game, _loggedInUserId)).Returns(gameModel);

        //Act
        var result = _controller.PlayAction(gameId, input) as OkObjectResult;

        //Assert
        Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");

        _gameServiceMock.Verify(
            service => service.PlayAction(gameId, _loggedInUserId, input.Cards, input.TargetPlayerId, input.TargetCard, input.DrawPileIndex),
            Times.Once,
            "The 'PlayAction' method of the game service is not called correctly");

        _mapperMock.Verify(mapper => mapper.MapGame(game, _loggedInUserId), Times.Once,
            "The game is not correctly mapped to a game model");

        Assert.That(result!.Value, Is.SameAs(gameModel), "The mapped game model should be in the OkObjectResult");
    }

    [MonitoredTest]
    public void DrawCard_ShouldUseGameServiceToDrawCard_ShouldReturnMappedModel()
    {
        //Arrange
        Guid gameId = Guid.NewGuid();
        IGame game = new GameMockBuilder().Object;
        _gameServiceMock.Setup(service => service.DrawCard(gameId, _loggedInUserId)).Returns(game);

        var gameModel = new GameModel();
        _mapperMock.Setup(mapper => mapper.MapGame(game, _loggedInUserId)).Returns(gameModel);

        //Act
        var result = _controller.DrawCard(gameId) as OkObjectResult;

        //Assert
        Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");

        _gameServiceMock.Verify(service => service.DrawCard(gameId, _loggedInUserId), Times.Once,
            "The 'DrawCard' method of the game service is not called correctly");

        _mapperMock.Verify(mapper => mapper.MapGame(game, _loggedInUserId), Times.Once,
            "The game is not correctly mapped to a game model");

        Assert.That(result!.Value, Is.SameAs(gameModel), "The mapped game model should be in the OkObjectResult");
    }

    [MonitoredTest]
    public void ConfirmNotNoping_ShouldUseGameServiceToConfirmNotNoping_ShouldReturnMappedModel()
    {
        //Arrange
        Guid gameId = Guid.NewGuid();
        IGame game = new GameMockBuilder().Object;
        _gameServiceMock.Setup(service => service.ConfirmNotNopingPendingAction(gameId, _loggedInUserId)).Returns(game);

        var gameModel = new GameModel();
        _mapperMock.Setup(mapper => mapper.MapGame(game, _loggedInUserId)).Returns(gameModel);

        //Act
        var result = _controller.ConfirmNotNoping(gameId) as OkObjectResult;

        //Assert
        Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");

        _gameServiceMock.Verify(service => service.ConfirmNotNopingPendingAction(gameId, _loggedInUserId), Times.Once,
            "The 'ConfirmNotNopingPendingAction' method of the game service is not called correctly");

        _mapperMock.Verify(mapper => mapper.MapGame(game, _loggedInUserId), Times.Once,
            "The game is not correctly mapped to a game model");

        Assert.That(result!.Value, Is.SameAs(gameModel), "The mapped game model should be in the OkObjectResult");
    }

    [MonitoredTest]
    public void Nope_ShouldUseGameServiceToNopePendingAction_ShouldReturnMappedModel()
    {
        //Arrange
        Guid gameId = Guid.NewGuid();
        IGame game = new GameMockBuilder().Object;
        _gameServiceMock.Setup(service => service.NopePendingAction(gameId, _loggedInUserId)).Returns(game);

        var gameModel = new GameModel();
        _mapperMock.Setup(mapper => mapper.MapGame(game, _loggedInUserId)).Returns(gameModel);

        //Act
        var result = _controller.Nope(gameId) as OkObjectResult;

        //Assert
        Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");

        _gameServiceMock.Verify(service => service.NopePendingAction(gameId, _loggedInUserId), Times.Once,
            "The 'NopePendingAction' method of the game service is not called correctly");

        _mapperMock.Verify(mapper => mapper.MapGame(game, _loggedInUserId), Times.Once,
            "The game is not correctly mapped to a game model");

        Assert.That(result!.Value, Is.SameAs(gameModel), "The mapped game model should be in the OkObjectResult");
    }

    [MonitoredTest]
    public void SelectCardToGiveAsAFavor_ShouldUseGameServiceToSelectCard_ShouldReturnMappedModel()
    {
        //Arrange
        Guid gameId = Guid.NewGuid();
        var input = new GiftModel { Card = Card.TacoCat };

        IGame game = new GameMockBuilder().Object;
        _gameServiceMock.Setup(service => service.SelectCardToGiveAsAFavor(gameId, _loggedInUserId, input.Card)).Returns(game);

        var gameModel = new GameModel();
        _mapperMock.Setup(mapper => mapper.MapGame(game, _loggedInUserId)).Returns(gameModel);

        //Act
        var result = _controller.SelectCardToGiveAsAFavor(gameId, input) as OkObjectResult;

        //Assert
        Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");

        _gameServiceMock.Verify(service => service.SelectCardToGiveAsAFavor(gameId, _loggedInUserId, input.Card), Times.Once,
            "The 'SelectCardToGiveAsAFavor' method of the game service is not called correctly");

        _mapperMock.Verify(mapper => mapper.MapGame(game, _loggedInUserId), Times.Once,
            "The game is not correctly mapped to a game model");

        Assert.That(result!.Value, Is.SameAs(gameModel), "The mapped game model should be in the OkObjectResult");
    }
}
