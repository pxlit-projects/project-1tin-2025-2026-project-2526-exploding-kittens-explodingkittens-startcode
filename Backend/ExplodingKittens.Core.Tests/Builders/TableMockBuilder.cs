using ExplodingKittens.Core.PlayerAggregate.Contracts;
using ExplodingKittens.Core.TableAggregate.Contracts;
using ExplodingKittens.Core.UserAggregate;
using Moq;

namespace ExplodingKittens.Core.Tests.Builders;

public class TableMockBuilder : MockBuilder<ITable>
{
    private ITablePreferences _tablePreferences = new TablePreferencesBuilder().Build();
    private readonly List<PlayerMockBuilder> _seatedPlayerMockBuilders = [];

    public IReadOnlyList<PlayerMockBuilder> SeatedPlayerMockBuilders => _seatedPlayerMockBuilders;

    public TableMockBuilder()
    {
        Mock.SetupGet(t => t.Id).Returns(Guid.NewGuid());
        Mock.SetupGet(t => t.SeatedPlayers).Returns([]);
        Mock.SetupGet(t => t.HasAvailableSeat).Returns(true);
        Mock.SetupGet(t => t.GameId).Returns(Guid.Empty);
        Mock.SetupGet(t => t.Preferences).Returns(() => _tablePreferences);
    }

    public TableMockBuilder WithPreferences(ITablePreferences tablePreferences)
    {
        _tablePreferences = tablePreferences;
        return this;
    }

    public TableMockBuilder WithSeatedUsers(User[] users)
    {
        IPlayer[] players = new IPlayer[users.Length];
        for (int i = 0; i < users.Length; i++)
        {
            var playerMockBuilder = new PlayerMockBuilder();
            _seatedPlayerMockBuilders.Add(playerMockBuilder);
            IPlayer player = playerMockBuilder
                .BasedOnUser(users[i])
                .Object;
            players[i] = player;
        }
        Mock.SetupGet(t => t.SeatedPlayers).Returns(() => players);
        Mock.Setup(table => table.Leave(It.IsAny<Guid>())).Callback((Guid playerId) =>
        {
            players = players.Where(p => p.Id != playerId).ToArray();
        });
        Mock.SetupGet(t => t.HasAvailableSeat).Returns(() => players.Length < _tablePreferences.NumberOfPlayers + _tablePreferences.NumberOfArtificialPlayers);

        return this;
    }
}