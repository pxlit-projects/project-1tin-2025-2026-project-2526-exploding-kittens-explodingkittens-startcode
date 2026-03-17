using ExplodingKittens.Core.TableAggregate.Contracts;

namespace ExplodingKittens.Core.GameAggregate.Contracts;

/// <summary>
/// Factory to create games
/// </summary>
public interface IGameFactory
{
    /// <summary>
    /// Creates a new game for a table.
    /// Steps include:
    /// * TODO
    /// * Generate a game id
    /// * TODO
    /// </summary>
    /// <param name="table">The table that wants to play a game</param>
    IGame CreateNewForTable(ITable table);
}