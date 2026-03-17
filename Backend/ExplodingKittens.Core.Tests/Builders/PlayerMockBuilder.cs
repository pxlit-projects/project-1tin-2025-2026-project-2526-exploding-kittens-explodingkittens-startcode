using ExplodingKittens.Core.CardAggregate;
using ExplodingKittens.Core.PlayerAggregate.Contracts;
using ExplodingKittens.Core.Tests.Extensions;
using ExplodingKittens.Core.UserAggregate;
using Moq;

namespace ExplodingKittens.Core.Tests.Builders;

public class PlayerMockBuilder : MockBuilder<IPlayer>
{
    private readonly HandMockBuilder _handMockBuilder;

    public HandMockBuilder HandMockBuilder => _handMockBuilder;
    public Mock<IHand> HandMock => _handMockBuilder.Mock;

    public PlayerMockBuilder()
    {
        Guid id = Guid.NewGuid();
        Mock.SetupGet(p => p.Id).Returns(id);
        Mock.SetupGet(p => p.Name).Returns("Player");
        Mock.SetupGet(p => p.BirthDate).Returns(Random.Shared.NextDateOnly(1980, 2015));
        Mock.SetupGet(p => p.Eliminated).Returns(false);
        Mock.SetupGet(p => p.FutureCards).Returns(new List<Card>());

        _handMockBuilder = new HandMockBuilder();
        Mock.SetupGet(p => p.Hand).Returns(_handMockBuilder.Object);
    }

    public PlayerMockBuilder BasedOnUser(User user)
    {
        Mock.SetupGet(p => p.Id).Returns(user.Id);
        Mock.SetupGet(p => p.Name).Returns(user.UserName ?? "Unknown");
        Mock.SetupGet(p => p.BirthDate).Returns(user.BirthDate);
        return this;
    }

    public PlayerMockBuilder WithName(string name)
    {
        Mock.SetupGet(p => p.Name).Returns(name);
        return this;
    }

    public PlayerMockBuilder WithBirthDate(DateOnly birthDate)
    {
        Mock.SetupGet(p => p.BirthDate).Returns(birthDate);
        return this;
    }

    public void WithEliminated(bool isEliminated)
    {
        Mock.SetupGet(p => p.Eliminated).Returns(isEliminated);
    }

    public void WithFutureCards(IReadOnlyList<Card> cards)
    {
        Mock.SetupGet(p => p.FutureCards).Returns(cards);
    }
}