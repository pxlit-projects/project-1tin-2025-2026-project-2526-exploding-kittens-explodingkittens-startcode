using ExplodingKittens.Core.CardAggregate.Contracts;

namespace ExplodingKittens.Core.CardAggregate;

/// <inheritdoc cref="ICardDeck"/>
internal class CardDeck
{
    /// <summary>
    /// Creates the deck
    /// </summary>
    /// <param name="cards">The first card is the card on top, the last card is the bottom card</param>
    public CardDeck(IList<Card> cards)
    {
        
    }
}