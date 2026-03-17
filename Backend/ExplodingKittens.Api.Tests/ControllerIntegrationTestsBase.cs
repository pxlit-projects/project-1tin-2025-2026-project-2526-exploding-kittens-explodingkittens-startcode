using System.Net.Http.Json;
using System.Text.Json;
using ExplodingKittens.Api.Controllers;
using ExplodingKittens.Api.Models.Input;
using ExplodingKittens.Api.Models.Output;
using ExplodingKittens.Api.Tests.Util;
using ExplodingKittens.Core.TableAggregate;
using ExplodingKittens.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExplodingKittens.Api.Tests;

public abstract class ControllerIntegrationTestsBase<TController> where TController : ApiControllerBase
{
    protected HttpClient ClientA = null!;
    protected HttpClient ClientB = null!;
    protected JsonSerializerOptions JsonSerializerOptions = null!;
    protected AccessPassModel PlayerAAccessPass = null!;
    protected AccessPassModel PlayerBAccessPass = null!;

    [OneTimeSetUp]
    public void BeforeAllTests()
    {
        JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        //JsonSerializerOptions.Converters.Add(new TwoDimensionalArrayJsonConverter<TileSpotModel>());

        var factory = new TestWebApplicationFactory(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<KittensDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Build configuration to read from testsettings.json and environment variables
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("testsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            // Replace the database with test database
            services.AddDbContext<KittensDbContext>(options =>
            {
                string connectionString = configuration.GetConnectionString("TestDatabase")!;
                options.UseSqlServer(connectionString).EnableSensitiveDataLogging(true);
            });

            // Make sure the test database is deleted before each test run
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<KittensDbContext>();
            dbContext.Database.EnsureDeleted();
        });
        ClientA = factory.CreateClient();
        ClientB = factory.CreateClient();

        PlayerAAccessPass = RegisterAndLoginUser(ClientA, "PlayerA", new DateOnly(2000, 1, 20));
        PlayerBAccessPass = RegisterAndLoginUser(ClientB, "PlayerB", new DateOnly(2005, 3, 15));
    }

    [OneTimeTearDown]
    public void AfterAllTests()
    {
        ClientA.Dispose();
        ClientB.Dispose();
    }

    private AccessPassModel RegisterAndLoginUser(HttpClient client, string userName, DateOnly birthDate)
    {
        var registerModel = new RegisterModel
        {
            UserName = userName,
            Email = $"{userName}@test.be",
            Password = "password",
            BirthDate = birthDate
        };
        client.PostAsJsonAsync("api/authentication/register", registerModel).Wait();
        HttpResponseMessage response = client.PostAsJsonAsync("api/authentication/token", new LoginModel
        {
            Email = registerModel.Email,
            Password = registerModel.Password
        }).Result!;

        AccessPassModel accessPassModel = response.Content.ReadFromJsonAsync<AccessPassModel>(JsonSerializerOptions).Result!;
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessPassModel.Token);
        return accessPassModel;
    }

    protected TableModel StartANewGameForANewTable()
    {
        //User A creates a table
        var tablePreferences = new TablePreferences();
        Assert.That(tablePreferences.NumberOfPlayers, Is.EqualTo(2), "The default number of players should be 2");
        Assert.That(tablePreferences.NumberOfArtificialPlayers, Is.EqualTo(0), "The default number of AI players should be 0");

        HttpResponseMessage response = ClientA.PostAsJsonAsync("api/tables", tablePreferences).Result;
        Assert.That((int)response.StatusCode, Is.EqualTo(StatusCodes.Status201Created), "User A could not correctly create a table.");
        TableModel table = response.Content.ReadFromJsonAsync<TableModel>(JsonSerializerOptions).Result!;
        Assert.That(table, Is.Not.Null, "User A could not correctly add a table.");
        Assert.That(table.SeatedPlayers.Count, Is.EqualTo(1), "User A could not correctly create a table. There should be 1 seated player");
        Assert.That(table.SeatedPlayers.First().Name, Is.EqualTo(PlayerAAccessPass.User.UserName),
            "User A could not correctly create a table. The seated player has an incorrect name");
        Assert.That(table.SeatedPlayers.First().Id, Is.EqualTo(PlayerAAccessPass.User.Id),
            "User A could not correctly create a table. The seated player has an incorrect id (should be the id of the user");
        Assert.That(table.GameId, Is.EqualTo(Guid.Empty),
            "User A could not correctly create a table. The GameId of the new table should be an empty Guid.");
        Assert.That(table.HasAvailableSeat, Is.True,
            "User A could not correctly create a table. The table should have available seats left.");
        Assert.That(table.Preferences.NumberOfPlayers, Is.EqualTo(tablePreferences.NumberOfPlayers),
            "User A could not correctly create a table. The table should have the preferences that were posted.");

        //User B joins the table
        response = ClientB.PostAsJsonAsync($"api/tables/{table.Id}/join", tablePreferences).Result;
        Assert.That((int)response.StatusCode, Is.EqualTo(StatusCodes.Status200OK), "User B could not correctly join the table.");
        table = response.Content.ReadFromJsonAsync<TableModel>(JsonSerializerOptions).Result!;
        Assert.That(table, Is.Not.Null, "User B could not correctly join the available table.");
        Assert.That(table.SeatedPlayers.Count, Is.EqualTo(2),
            "User B could not correctly join the available table. There should be 2 seated players");
        Assert.That(table.SeatedPlayers.First().Name, Is.Not.EqualTo(table.SeatedPlayers.Last().Name),
                       "User B could not correctly join the available table. The seated players should have different names");
        Assert.That(table.HasAvailableSeat, Is.False,
            "User B could not correctly join the available table. The table should not have any available seats left.");
        Assert.That(table.GameId, Is.Not.EqualTo(Guid.Empty),
            "When the table is full, a game should be started, but the Game Id is empty");
        return table;
    }
}