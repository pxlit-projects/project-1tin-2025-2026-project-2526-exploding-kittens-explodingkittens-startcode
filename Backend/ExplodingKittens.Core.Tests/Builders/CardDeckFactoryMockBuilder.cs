using ExplodingKittens.Core.CardAggregate.Contracts;
using Moq;

namespace ExplodingKittens.Core.Tests.Builders;

public class CardDeckFactoryMockBuilder : MockBuilder<ICardDeckFactory>
{
    private readonly CardDeckMockBuilder _cardDeckMockBuilder;

    public Mock<ICardDeck> CardDeckMock => _cardDeckMockBuilder.Mock;

    public int LastCreatedDeckCardCount { get; private set; }

    public CardDeckFactoryMockBuilder()
    {
        _cardDeckMockBuilder = new CardDeckMockBuilder();
        Mock.Setup(f => f.CreateStandardDeckWithoutExplodingKittens(It.IsAny<int>())).Returns(_cardDeckMockBuilder.Object).Callback(() =>
        {
            LastCreatedDeckCardCount = _cardDeckMockBuilder.Object.CardCount;
        }); 
    }
}