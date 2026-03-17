using ExplodingKittens.Api.Models.Output;
using ExplodingKittens.Core.ActionAggregate.Contracts;
using ExplodingKittens.Core.GameAggregate.Contracts;
using ExplodingKittens.Core.PlayerAggregate.Contracts;
using ExplodingKittens.Core.TableAggregate.Contracts;
using ExplodingKittens.Core.UserAggregate;
using Riok.Mapperly.Abstractions;

namespace ExplodingKittens.Api.Models;

[Mapper]
internal partial class ModelMapper : IModelMapper
{
    public partial UserModel MapUser(User user);
    public partial PlayerModel MapPlayer(IPlayer player);
    public partial TableModel MapTable(ITable table);

    public GameModel MapGame(IGame game, Guid viewingPlayerId)
    {
        List<PlayerInGameModel> players = game.Players
            .Select(p => MapPlayerInGameWithCards(p, p.Id == viewingPlayerId))
            .ToList();

        return new GameModel
        {
            Id = game.Id,
            Players = players,
            DiscardPile = game.DiscardPile.ToList(),
            DrawPileCount = game.DrawPile.CardCount,
            PlayerToPlayId = game.PlayerToPlayId,
            PendingDraws = game.PendingDraws,
            PendingAction = MapAction(game.PendingAction),
            HasEnded = game.HasEnded
        };
    }

    private PlayerInGameModel MapPlayerInGameWithCards(IPlayer player, bool showCards)
    {
        return new PlayerInGameModel
        {
            Id = player.Id,
            Name = player.Name,
            BirthDate = player.BirthDate,
            HasExplodingKitten = player.HasExplodingKitten,
            Eliminated = player.Eliminated,
            FutureCards = showCards ? player.FutureCards.ToList() : [],
            CardsInHandCount = player.Hand.Cards.Count,
            CardsInHand = showCards ? player.Hand.Cards.ToList() : []
        };
    }

    private partial ActionModel? MapAction(IAction? action);
}