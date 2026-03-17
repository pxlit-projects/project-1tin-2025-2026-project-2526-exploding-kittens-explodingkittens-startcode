using ExplodingKittens.Core.GameAggregate.Contracts;

namespace ExplodingKittens.Core.Tests.Builders;

public class GameMockBuilder : MockBuilder<IGame>
{
    public GameMockBuilder()
    {
        Mock.SetupGet(g => g.Id).Returns(Guid.NewGuid());

        var player1 = new PlayerMockBuilder().Object;
        var player2 = new PlayerMockBuilder().Object;

        Mock.SetupGet(g => g.Players).Returns([player1, player2]);    
        Mock.Setup(g => g.GetPlayerById(player1.Id)).Returns(player1);
        Mock.Setup(g => g.GetPlayerById(player2.Id)).Returns(player2);
        Mock.SetupGet(g => g.DrawPile).Returns(new CardDeckMockBuilder().Object);
    }
}