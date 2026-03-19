using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Projekthantering.Services;
using Projekthantering.Shared.DTOs;

namespace Projekthantering.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoardsController : ControllerBase
{
    private readonly IBoardService _boardService;

    public BoardsController(IBoardService boardService)
    {
        _boardService = boardService;
    }

    // GET /api/boards
    [HttpGet]
    public async Task<ActionResult<List<BoardDto>>> GetBoards()
    {
        var boards = await _boardService.GetBoardsAsync();
        return Ok(boards);
    }

    // GET /api/boards/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<BoardDto>> GetBoard(int id)
    {
        var board = await _boardService.GetBoardByIdAsync(id);
        if (board is null)
            return NotFound(new { message = "Tavlan hittades inte." });

        return Ok(board);
    }

    // POST /api/boards
    [HttpPost]
    public async Task<ActionResult<BoardDto>> CreateBoard(CreateBoardRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Efter — läser userId ur JWT-claimet
        var ownerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(ownerIdClaim, out var ownerId))
            return Unauthorized();
        var board = await _boardService.CreateBoardAsync(request, ownerId);
        return CreatedAtAction(nameof(GetBoard), new { id = board.Id }, board);
    }

    // PATCH /api/boards/{id}
    [HttpPatch("{id}")]
    public async Task<ActionResult<BoardDto>> UpdateBoard(int id, UpdateBoardRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var board = await _boardService.UpdateBoardAsync(id, request);
        if (board is null)
            return NotFound(new { message = "Tavlan hittades inte." });

        return Ok(board);
    }

    // DELETE /api/boards/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBoard(int id)
    {
        var deleted = await _boardService.DeleteBoardAsync(id);
        if (!deleted)
            return NotFound(new { message = "Tavlan hittades inte." });

        return NoContent();
    }
}

