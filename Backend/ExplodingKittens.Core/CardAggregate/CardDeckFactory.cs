using ExplodingKittens.Core.CardAggregate.Contracts;

namespace ExplodingKittens.Core.CardAggregate;

/// <inheritdoc cref="ICardDeckFactory"/>
internal class CardDeckFactory : ICardDeckFactory
{
    public ICardDeck CreateStandardDeckWithoutExplodingKittens(int numberOfPlayers)
    {
        throw new NotImplementedException();
    }
}