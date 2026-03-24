using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projekthantering.Services;
using Projekthantering.Shared.DTOs;

namespace Projekthantering.Controllers;

[ApiController]
[Authorize]
public class CardsController : ControllerBase
{
    private readonly ICardService _cardService;

    public CardsController(ICardService cardService)
    {
        _cardService = cardService;
    }

    // POST /api/lists/{listId}/cards
    [HttpPost("api/lists/{listId}/cards")]
    public async Task<ActionResult<CardDto>> CreateCard(int listId, CreateCardRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var listExists = await _cardService.ListExistsAsync(listId);
        if (!listExists)
            return NotFound(new { message = "Listan hittades inte." });

        var card = await _cardService.CreateCardAsync(listId, request);
        return CreatedAtAction(nameof(GetCard), new { id = card.Id }, card);
    }

    // GET /api/lists/{listId}/cards
    [HttpGet("api/lists/{listId}/cards")]
    public async Task<ActionResult<List<CardDto>>> GetCardsByList(int listId)
    {
        var listExists = await _cardService.ListExistsAsync(listId);
        if (!listExists)
            return NotFound(new { message = "Listan hittades inte." });

        var cards = await _cardService.GetCardsByListAsync(listId);
        return Ok(cards);
    }

    // PATCH /api/cards/{id}
    [HttpPatch("api/cards/{id}")]
    public async Task<ActionResult<CardDto>> UpdateCard(int id, UpdateCardRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var card = await _cardService.UpdateCardAsync(id, request);
        if (card is null)
            return NotFound(new { message = "Kortet hittades inte." });

        return Ok(card);
    }

    // DELETE /api/cards/{id}
    [HttpDelete("api/cards/{id}")]
    public async Task<IActionResult> DeleteCard(int id)
    {
        var deleted = await _cardService.DeleteCardAsync(id);
        if (!deleted)
            return NotFound(new { message = "Kortet hittades inte." });

        return NoContent();
    }

    // Helper action used by CreatedAtAction in CreateCard
    [HttpGet("api/cards/{id}")]
    public async Task<ActionResult<CardDto>> GetCard(int id)
    {
        var card = await _cardService.GetCardByIdAsync(id);
        if (card is null)
            return NotFound(new { message = "Kortet hittades inte." });

        return Ok(card);
    }
}
