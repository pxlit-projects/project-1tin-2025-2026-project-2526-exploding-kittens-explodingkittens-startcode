using ExplodingKittens.Api.Models.Output;
using ExplodingKittens.Core.GameAggregate.Contracts;
using ExplodingKittens.Core.PlayerAggregate.Contracts;
using ExplodingKittens.Core.TableAggregate.Contracts;
using ExplodingKittens.Core.UserAggregate;

namespace ExplodingKittens.Api.Models;

public interface IModelMapper
{
    UserModel MapUser(User user);
    PlayerModel MapPlayer(IPlayer player);
    TableModel MapTable(ITable table);

    /// <summary>Maps the game for a specific viewer; only that player sees their own cards in hand; others see only card count.</summary>
    GameModel MapGame(IGame game, Guid viewingPlayerId);
}