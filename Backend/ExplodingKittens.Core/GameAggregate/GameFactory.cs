using ExplodingKittens.Core.ActionAggregate.Contracts;
using ExplodingKittens.Core.CardAggregate.Contracts;
using ExplodingKittens.Core.GameAggregate.Contracts;
using ExplodingKittens.Core.TableAggregate.Contracts;

namespace ExplodingKittens.Core.GameAggregate;

internal class GameFactory : IGameFactory
{
    public GameFactory(ICardDeckFactory cardDeckFactory, IActionFactory actionFactory)
    {
    }

    public IGame CreateNewForTable(ITable table)
    {
        throw new NotImplementedException();
    }
}