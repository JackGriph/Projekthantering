using Microsoft.AspNetCore.Mvc;
using Projekthantering.Services;
using Projekthantering.Shared.DTOs;

namespace Projekthantering.Controllers;

[ApiController]
public class ListsController : ControllerBase
{
    private readonly IListService _listService;

    public ListsController(IListService listService)
    {
        _listService = listService;
    }

    // POST /api/boards/{boardId}/lists
    [HttpPost("api/boards/{boardId}/lists")]
    public async Task<ActionResult<BoardListDto>> CreateList(int boardId, CreateListRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var boardExists = await _listService.BoardExistsAsync(boardId);
        if (!boardExists)
            return NotFound(new { message = "Tavlan hittades inte." });

        var list = await _listService.CreateListAsync(boardId, request);
        return CreatedAtAction(nameof(GetList), new { id = list.Id }, list);
    }

    // GET /api/boards/{boardId}/lists
    [HttpGet("api/boards/{boardId}/lists")]
    public async Task<ActionResult<List<BoardListDto>>> GetListsByBoard(int boardId)
    {
        var boardExists = await _listService.BoardExistsAsync(boardId);
        if (!boardExists)
            return NotFound(new { message = "Tavlan hittades inte." });

        var lists = await _listService.GetListsByBoardAsync(boardId);
        return Ok(lists);
    }

    // PATCH /api/lists/{id}
    [HttpPatch("api/lists/{id}")]
    public async Task<ActionResult<BoardListDto>> UpdateList(int id, UpdateListRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var list = await _listService.UpdateListAsync(id, request);
        if (list is null)
            return NotFound(new { message = "Listan hittades inte." });

        return Ok(list);
    }

    // DELETE /api/lists/{id}
    [HttpDelete("api/lists/{id}")]
    public async Task<IActionResult> DeleteList(int id)
    {
        var deleted = await _listService.DeleteListAsync(id);
        if (!deleted)
            return NotFound(new { message = "Listan hittades inte." });

        return NoContent();
    }

    // Helper action used by CreatedAtAction in CreateList
    [HttpGet("api/lists/{id}")]
    public async Task<ActionResult<BoardListDto>> GetList(int id)
    {
        var list = await _listService.GetListByIdAsync(id);
        if (list is null)
            return NotFound(new { message = "Listan hittades inte." });

        return Ok(list);
    }
}
