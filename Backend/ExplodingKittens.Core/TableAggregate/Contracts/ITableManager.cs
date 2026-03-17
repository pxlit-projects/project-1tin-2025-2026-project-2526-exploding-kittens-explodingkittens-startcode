using ExplodingKittens.Core.GameAggregate.Contracts;
using ExplodingKittens.Core.UserAggregate;

namespace ExplodingKittens.Core.TableAggregate.Contracts;

/// <summary>
/// Manages all the tables of the application
/// </summary>
public interface ITableManager
{
    /// <summary>
    /// Creates a new table with the given preferences and adds the user to it.
    /// </summary>
    /// <param name="user">The user creating the table.</param>
    /// <param name="preferences">The preferences for the new table.</param>
    /// <returns>The created table.</returns>
    ITable CreateTable(User user, ITablePreferences preferences);


    /// <summary>
    /// Adds a user a table.
    /// </summary>
    /// <param name="tableId">The unique identifier of the table to join.</param>
    /// <param name="user">The user who is joining the table.</param>
    /// <returns>An <see cref="ITable"/> instance representing the joined table.</returns>
    ITable JoinTable(Guid tableId, User user);

    /// <summary>
    /// Removes a user from a table.
    /// If the table has no players left, it is removed from the system.
    /// </summary>
    void LeaveTable(Guid tableId, User user);

    /// <summary>
    /// Starts a game for a table.
    /// </summary>
    IGame StartGameForTable(Guid tableId);
}