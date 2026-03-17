using ExplodingKittens.Core.TableAggregate;

namespace ExplodingKittens.Api.Models.Output;

public class TableModel
{
    public Guid Id { get; set; }
    public TablePreferences Preferences { get; set; } = new();
    public List<PlayerModel> SeatedPlayers { get; set; } = [];
    public bool HasAvailableSeat { get; set; }
    public Guid GameId { get; set; }
}