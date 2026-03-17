using ExplodingKittens.Core.CardAggregate;
using ExplodingKittens.Core.PlayerAggregate.Contracts;
using Moq;

namespace ExplodingKittens.Core.Tests.Builders;

public class HandMockBuilder : MockBuilder<IHand>
{
    private readonly List<Card> _cards;

    public HandMockBuilder()
    {
        _cards = [];
       Mock.SetupGet(h => h.Cards).Returns(_cards);
       Mock.Setup(h => h.PickSpecificCard(It.IsAny<Card>())).Returns((Card c) => _cards.Contains(c) ? c : null);
       Mock.Setup(h => h.PickRandomCard())
           .Returns(() =>
           {
               Card? card = _cards.Count > 0 ? _cards[Random.Shared.Next(_cards.Count)] : null;
               if (card is not null)
               {
                   _cards.Remove(card.Value);
               }
               return card;
           });
    }

    public HandMockBuilder WithCard(Card card)
    {
        _cards.Add(card);
        return this;
    }

}