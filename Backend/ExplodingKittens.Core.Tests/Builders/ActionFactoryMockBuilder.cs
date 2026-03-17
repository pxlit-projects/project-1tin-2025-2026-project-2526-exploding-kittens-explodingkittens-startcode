using ExplodingKittens.Core.ActionAggregate.Contracts;
using ExplodingKittens.Core.CardAggregate;
using ExplodingKittens.Core.GameAggregate.Contracts;
using Moq;

namespace ExplodingKittens.Core.Tests.Builders;

public class ActionFactoryMockBuilder : MockBuilder<IActionFactory>
{
    private readonly ActionMockBuilder _actionMockBuilder;

    public Mock<IAction> ActionMock => _actionMockBuilder.Mock;

    public ActionFactoryMockBuilder()
    {
        _actionMockBuilder = new ActionMockBuilder();
        Mock.Setup(f => f.Create(
                It.IsAny<IGame>(), 
                It.IsAny<Guid>(), 
                It.IsAny<IReadOnlyList<Card>>(),
                It.IsAny<Guid?>(), 
                It.IsAny<Card?>(), 
                It.IsAny<int?>()))
            .Returns(() => _actionMockBuilder.Object);
    }

    public ActionFactoryMockBuilder WithCreateReturnsUnexecutedAction(Card card)
    {
        _actionMockBuilder.WithCard(card).WithExecuted(false);
        return this;
    }

    public ActionFactoryMockBuilder WithCreateReturnsUnexecutedAction(IAction action)
    {
        Mock.Setup(f => f.Create(
                It.IsAny<IGame>(),
                It.IsAny<Guid>(),
                It.IsAny<IReadOnlyList<Card>>(),
                It.IsAny<Guid?>(),
                It.IsAny<Card?>(),
                It.IsAny<int?>()))
            .Returns(() => action);
        return this;
    }

    public ActionFactoryMockBuilder WithCreateReturnsExecutedAction(Card card)
    {
        _actionMockBuilder.WithCard(card).WithExecuted(true);
        return this;
    }

   
}