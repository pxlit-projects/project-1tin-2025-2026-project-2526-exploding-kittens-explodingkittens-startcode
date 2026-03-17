using ExplodingKittens.Core.CardAggregate;

namespace ExplodingKittens.Core.ActionAggregate.Contracts;

/// <summary>
/// Represents the play of a card or a combination of cards by a player, which may be 'noped' by other players.
/// </summary>
public interface IAction
{
    /// <summary>
    /// Id of the player who initiated this action
    /// </summary>
    Guid PlayerId { get; }

    IReadOnlyList<Card> Cards { get; }

    /// <summary>
    /// Indicates whether this move can be 'noped'
    /// </summary>
    bool CanBeNoped { get; }

    /// <summary>
    /// If not null, the action needs a target player
    /// </summary>
    Guid? TargetPlayerId { get; }

    /// <summary>
    /// If not null, the action needs a target card
    /// </summary>
    Card? TargetCard { get; set; }

    /// <summary>
    /// If not null, the action includes inserting a card (e.g. an exploding kitten) back into the draw pile at the specified index (0-based, where 0 means the top of the draw pile).
    /// This is used for actions like 'Defuse'.
    /// </summary>
    int? DrawPileIndex { get; }

    IReadOnlyDictionary<Guid, NopeDecision> PlayerNopeDecisions { get; }

    /// <summary>
    /// TRUE if at least one player has 'noped' this action (even if there are still undecided players).
    /// FALSE otherwise.
    /// </summary>
    bool IsNoped { get; }

    /// <summary>
    /// TRUE if the action has been executed.
    /// ALSO TRUE if the action is 'noped' and all players have confirmed that they are not 'noping' it.
    /// FALSE otherwise.
    /// </summary>
    bool IsExecuted { get; }

    /// <summary>
    /// When the action is not 'noped' yet:
    ///    - Records that the specified player 'Nopes' this action.
    ///    - Resets the 'Nope' decisions of all other players
    /// When the action is already 'noped', the 'nope' should be undone and thus:
    ///    - Records that the specified player is not 'Noping' this action.
    ///    - Resets the 'Nope' decisions of all other players.
    /// </summary>
    void Nope(Guid nopingPlayerId);

    /// <summary>
    /// Confirms that the specified player chooses not to 'Nope' this action.
    /// </summary>
    /// <param name="notNopingPlayerId">Unique identifier of a player</param>
    void ConfirmNotNoping(Guid notNopingPlayerId);


}