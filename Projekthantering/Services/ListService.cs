using Projekthantering.Models;
using Projekthantering.Repositories;
using Projekthantering.Shared.DTOs;

namespace Projekthantering.Services;

public class ListService : IListService
{
    private readonly IListRepository _listRepository;

    public ListService(IListRepository listRepository)
    {
        _listRepository = listRepository;
    }

    public async Task<List<BoardListDto>> GetListsByBoardAsync(int boardId)
    {
        var lists = await _listRepository.GetByBoardIdAsync(boardId);
        return lists.Select(MapToDto).ToList();
    }

    public async Task<BoardListDto?> GetListByIdAsync(int id)
    {
        var list = await _listRepository.GetByIdAsync(id);
        return list is null ? null : MapToDto(list);
    }

    public async Task<BoardListDto> CreateListAsync(int boardId, CreateListRequest request)
    {
        var existingLists = await _listRepository.GetByBoardIdAsync(boardId);
        var nextPosition = existingLists.Count > 0
            ? existingLists.Max(l => l.Position) + 1
            : 0;

        var list = new BoardList
        {
            Title = request.Title,
            BoardId = boardId,
            Position = nextPosition
        };

        var created = await _listRepository.CreateAsync(list);
        return MapToDto(created);
    }

    public async Task<BoardListDto?> UpdateListAsync(int id, UpdateListRequest request)
    {
        var list = await _listRepository.GetByIdAsync(id);
        if (list is null) return null;

        list.Title = request.Title;
        list.Position = request.Position;

        var updated = await _listRepository.UpdateAsync(list);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteListAsync(int id)
        => await _listRepository.DeleteAsync(id);

    private static BoardListDto MapToDto(BoardList list) => new()
    {
        Id = list.Id,
        Title = list.Title,
        Position = list.Position,
        Cards = list.Cards.Select(c => new CardDto
        {
            Id           = c.Id,
            Title        = c.Title,
            Description  = c.Description,
            ListId       = c.ListId,
            Position     = c.Position,
            Status       = c.Status,
            AssigneeId   = c.AssigneeId,
            AssigneeName = c.Assignee?.Username,
            DueDate      = c.DueDate,
            CreatedAt    = c.CreatedAt
        }).ToList()
    };
}
