using System.Net;
using System.Net.Http.Json;
using ExplodingKittens.Api.Controllers;
using ExplodingKittens.Api.Models.Input;
using ExplodingKittens.Api.Models.Output;
using ExplodingKittens.Core.CardAggregate;
using Guts.Client.NUnit;

namespace ExplodingKittens.Api.Tests;

[ProjectComponentTestFixture("1TINProject", "ExplodingKittens", "GamesCtrlIntegration",
    @"ExplodingKittens.Api\Controllers\GamesController.cs;ExplodingKittens.Api\Models\ModelMapper.cs")]
public class GamesControllerIntegrationTests : ControllerIntegrationTestsBase<GamesController>
{
    [MonitoredTest]
    public void _01_GetGame_JustAfterCreation_ShouldReturnAGameWithACorrectStartSituation()
    {
        TableModel table = StartANewGameForANewTable();

        GameModel game = GetGame(ClientA, table.GameId);
        Assert.That(game.Id, Is.EqualTo(table.GameId), "The returned game has an incorrect 'Id'");
        Assert.That(game.DiscardPile, Has.Count.Zero, "The returned game should have an empty 'DiscardPile'");
        Assert.That(game.DrawPileCount, Is.GreaterThan(20), "The draw pile should contain at least 20 cards (DrawPileCount)");
        Assert.That(game.DrawPileCount, Is.LessThan(34),
            "The draw pile should contain less than 34 cards (DrawPileCount). " +
            "When there are 2 or 3 players, one third of the cards should be removed before inserting the exploding kitten(s).");
        Assert.That(game.PendingDraws, Is.EqualTo(1), "There should be one pending draw (PendingDraws)");
        Assert.That(game.PendingAction, Is.Null, "There should be no pending action (PendingAction)");
        Assert.That(game.HasEnded, Is.False, "The game should not have ended yet (HasEnded)");


        //Players
        Assert.That(game.Players.Count, Is.EqualTo(2), "The game should have 2 players");

        var youngestPlayer = table.SeatedPlayers.OrderByDescending(p => p.BirthDate).First();
        Assert.That(game.PlayerToPlayId, Is.EqualTo(youngestPlayer.Id), "'PlayerToPlayId' should be the 'Id' of the youngest player");

        AssertIsValidPlayerAfterCreation(PlayerAAccessPass.User, game, PlayerAAccessPass.User);
        AssertIsValidPlayerAfterCreation(PlayerBAccessPass.User, game, PlayerAAccessPass.User);
    }

    [MonitoredTest]
    public void _02_PlayGame_PlayersOnlyDrawCards_GameShouldEventuallyEnd()
    {
        // Arrange
        TableModel table = StartANewGameForANewTable();
        GameModel game = GetGame(ClientA, table.GameId);

        int maxIterations = 200; // safety limit to prevent an infinite loop
        int iterations = 0;

        // Act - keep taking turns until the game ends
        while (!game.HasEnded && iterations < maxIterations)
        {
            HttpClient currentClient = game.PlayerToPlayId == PlayerAAccessPass.User.Id ? ClientA : ClientB;
            PlayerInGameModel currentPlayer = game.Players.First(p => p.Id == game.PlayerToPlayId);

            if (currentPlayer.HasExplodingKitten && !currentPlayer.Eliminated)
            {
                // Player drew an exploding kitten but has a defuse card — must play defuse to continue
                var defuseInput = new CreateActionModel
                {
                    Cards = new List<Card> { Card.Defuse },
                    TargetCard = Card.ExplodingKitten,
                    DrawPileIndex = 0
                };
                game = PlayAction(currentClient, game.Id, defuseInput);
            }
            else
            {
                game = DrawCard(currentClient, game.Id);
            }

            iterations++;
        }

        // Assert
        Assert.That(iterations, Is.LessThan(maxIterations), "The game did not end within the expected number of turns.");
        Assert.That(game.HasEnded, Is.True, "The game should have ended (HasEnded).");

        var activePlayers = game.Players.Where(p => !p.Eliminated).ToList();
        Assert.That(activePlayers, Has.Count.EqualTo(1), "Exactly one player should remain at the end of the game.");

        var eliminatedPlayers = game.Players.Where(p => p.Eliminated).ToList();
        Assert.That(eliminatedPlayers, Has.Count.EqualTo(1), "Exactly one player should be eliminated at the end of a 2-player game.");
    }

    private GameModel DrawCard(HttpClient client, Guid gameId)
    {
        var result = client.PostAsync($"api/games/{gameId}/draw-card", null).Result;
        ErrorModel? error = null;
        if (!result.IsSuccessStatusCode)
        {
            error = result.Content.ReadFromJsonAsync<ErrorModel>(JsonSerializerOptions).Result;
        }
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK),
            $"Error drawing a card for game '{gameId}'.\n {error?.Message}");

        GameModel? gameModel = result.Content.ReadFromJsonAsync<GameModel>(JsonSerializerOptions).Result;
        Assert.That(gameModel, Is.Not.Null, $"Error drawing a card for game '{gameId}'.\n Game model not found in response.");
        return gameModel!;
    }

    private GameModel PlayAction(HttpClient client, Guid gameId, CreateActionModel input)
    {
        var result = client.PostAsJsonAsync($"api/games/{gameId}/play-action", input).Result;
        ErrorModel? error = null;
        if (!result.IsSuccessStatusCode)
        {
            error = result.Content.ReadFromJsonAsync<ErrorModel>(JsonSerializerOptions).Result;
        }
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK),
            $"Error playing action for game '{gameId}'.\n {error?.Message}");

        GameModel? gameModel = result.Content.ReadFromJsonAsync<GameModel>(JsonSerializerOptions).Result;
        Assert.That(gameModel, Is.Not.Null, $"Error playing action for game '{gameId}'.\n Game model not found in response.");
        return gameModel!;
    }

    private GameModel GetGame(HttpClient client, Guid gameId)
    {
        var result = client.GetAsync($"api/games/{gameId}").Result;
        ErrorModel? error = null;
        if (!result.IsSuccessStatusCode)
        {
            error = result.Content.ReadFromJsonAsync<ErrorModel>(JsonSerializerOptions).Result;
        }
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK),
            $"Error retrieving the game with id '{gameId}'.\n {error?.Message}");

        GameModel? gameModel = result.Content.ReadFromJsonAsync<GameModel>(JsonSerializerOptions).Result;
        Assert.That(gameModel, Is.Not.Null, $"Error retrieving the game with id '{gameId}'.\n Game not found in Api response");
        return gameModel!;
    }

    private PlayerInGameModel AssertIsValidPlayerAfterCreation(UserModel user, GameModel game, UserModel viewingUser)
    {
        PlayerInGameModel? player = game.Players.FirstOrDefault(p => p.Id == user.Id);
        Assert.That(player, Is.Not.Null, $"Player for user '{user.UserName}' not found in the game");

        string playerName = player!.Name;
        Assert.That(playerName, Is.EqualTo(user.UserName), $"The name of the player should be the user name ({user.UserName})");
        Assert.That(player.BirthDate, Is.EqualTo(user.BirthDate), $"Player '{playerName}' should have the same birth date as its matching user");
        Assert.That(player.HasExplodingKitten, Is.False, $"Player '{playerName}' should not be marked as 'HasExplodingKitten'");
        Assert.That(player.Eliminated, Is.False, $"Player '{playerName}' should not be eliminated");
        Assert.That(player.FutureCards, Is.Not.Null, $"Player '{playerName}' should have a list to hold future cards");
        Assert.That(player.FutureCards, Has.Count.Zero, $"The list of future cards for player '{playerName}' should be empty");

        Assert.That(player.CardsInHandCount, Is.EqualTo(8), $"Player '{playerName}' should have 8 cards in hand (CardsInHandCount)");
        Assert.That(player.CardsInHand, Is.Not.Null, $"Player '{playerName}' should have a list to hold the cards in hand");

        bool canViewCardsInHand = viewingUser.Id == player.Id;
        if (canViewCardsInHand)
        {
            Assert.That(player.CardsInHand, Has.Count.EqualTo(8), $"Player '{playerName}' should have 8 cards in hand (CardsInHand)");
            bool hasExplodingKitten = player.CardsInHand.Any(card => card == Card.ExplodingKitten);
            Assert.That(hasExplodingKitten, Is.False, $"Player '{playerName}' should not have an 'Exploding Kitten' in hand at the start of the game");
            int defuseCount = player.CardsInHand.Count(card => card == Card.Defuse);
            Assert.That(defuseCount, Is.GreaterThanOrEqualTo(1), $"Player '{playerName}' should have at least 1 'Defuse' card in hand at the start of the game");
        }
        else
        {
            Assert.That(player.CardsInHand, Has.Count.Zero, $"Player '{playerName}' should not have any cards in hand (CardsInHand), because the user '{viewingUser.UserName}' is not the owner of these cards");
        }

        return player;
    }
}