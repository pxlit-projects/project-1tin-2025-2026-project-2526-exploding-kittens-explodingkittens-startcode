using ExplodingKittens.Api.Models;
using ExplodingKittens.Api.Models.Input;
using ExplodingKittens.Api.Models.Output;
using ExplodingKittens.Core.GameAggregate.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ExplodingKittens.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController : ApiControllerBase
{
    private readonly IGameService _gameService;
    private readonly IModelMapper _mapper;

    public GamesController(IGameService gameService, IModelMapper mapper)
    {
        _gameService = gameService;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets a game by id. The current user sees their own cards in hand; other players are shown only their card count.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GameModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        IGame game = _gameService.GetGame(id);
        GameModel model = _mapper.MapGame(game, UserId);
        return Ok(model);
    }

    /// <summary>
    /// Plays an action by using cards from the current player's hand.
    /// The pending action is not executed immediately.
    /// Other players must decide whether to 'Nope' the action or not.
    /// If no one 'Nopes' the action, the action is executed.
    /// </summary>
    [HttpPost("{id}/play-action")]
    [ProducesResponseType(typeof(GameModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    public IActionResult PlayAction(Guid id, [FromBody] CreateActionModel input)
    {
        IGame game = _gameService.PlayAction(id, UserId, input.Cards, input.TargetPlayerId, input.TargetCard, input.DrawPileIndex);
        GameModel output = _mapper.MapGame(game, UserId);
        return Ok(output);
    }

    /// <summary>
    /// Draws a card from the draw pile and adds it to the player's hand.
    /// If the drawn card is an 'Exploding Kitten', the player loses unless they have a 'Defuse' card to play.
    /// </summary>
    [HttpPost("{id}/draw-card")]
    [ProducesResponseType(typeof(GameModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    public IActionResult DrawCard(Guid id)
    {
        IGame game = _gameService.DrawCard(id, UserId);
        GameModel output = _mapper.MapGame(game, UserId);
        return Ok(output);
    }

    /// <summary>
    /// Confirms that the current player is not 'Noping' the pending action.
    /// </summary>
    [HttpPost("{id}/confirm-not-noping")]
    [ProducesResponseType(typeof(GameModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    public IActionResult ConfirmNotNoping(Guid id)
    {
        IGame game = _gameService.ConfirmNotNopingPendingAction(id, UserId);
        GameModel output = _mapper.MapGame(game, UserId);
        return Ok(output);
    }

    /// <summary>
    /// Nopes the pending action
    /// </summary>
    [HttpPost("{id}/nope")]
    [ProducesResponseType(typeof(GameModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    public IActionResult Nope(Guid id)
    {
        IGame game = _gameService.NopePendingAction(id, UserId);
        GameModel output = _mapper.MapGame(game, UserId);
        return Ok(output);
    }

    /// <summary>
    /// Selects a card from the current player's hand to give as a favor to another player that played a 'Favor' card.
    /// </summary>
    [HttpPost("{id}/select-card-to-give-as-a-favor")]
    [ProducesResponseType(typeof(GameModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    public IActionResult SelectCardToGiveAsAFavor(Guid id,[FromBody] GiftModel input)
    {
        IGame game = _gameService.SelectCardToGiveAsAFavor(id, UserId, input.Card);
        GameModel output = _mapper.MapGame(game, UserId);
        return Ok(output);
    }
}
