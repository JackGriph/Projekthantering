using Projekthantering.Models;
using Projekthantering.Repositories;
using Projekthantering.Shared.DTOs;


namespace Projekthantering.Services;

public class BoardService : IBoardService
{
    private readonly IBoardRepository _boardRepository;

    public BoardService(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task<List<BoardDto>> GetBoardsAsync()
    {
        var boards = await _boardRepository.GetAllAsync();
        return boards.Select(MapToDto).ToList();
    }

    public async Task<List<BoardDto>> GetBoardsByUserAsync(int userId)
    {
        var boards = await _boardRepository.GetByUserIdAsync(userId);
        return boards.Select(MapToDto).ToList();
    }

    public async Task<BoardDto?> GetBoardByIdAsync(int id, int userId)
    {
        var board = await _boardRepository.GetByIdAsync(id);
        if (board is null) return null;

        var hasAccess = board.OwnerId == userId || board.Members.Any(m => m.UserId == userId);
        if (!hasAccess) return null;

        return MapToDto(board);
    }

    public async Task<BoardDto> CreateBoardAsync(CreateBoardRequest request, int ownerId)
    {
        var board = new Board
        {
            Title = request.Title,
            Description = request.Description,
            OwnerId = ownerId
        };

        var created = await _boardRepository.CreateAsync(board);
        return MapToDto(created);
    }

    public async Task<BoardDto?> UpdateBoardAsync(int id, UpdateBoardRequest request, int userId)
    {
        var board = await _boardRepository.GetByIdAsync(id);
        if (board is null) return null;
        if (board.OwnerId != userId) return null;

        board.Title = request.Title;
        board.Description = request.Description;

        var updated = await _boardRepository.UpdateAsync(board);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteBoardAsync(int id, int userId)
    {
        var board = await _boardRepository.GetByIdAsync(id);
        if (board is null) return false;
        if (board.OwnerId != userId) return false;

        var delete = await _boardRepository.DeleteAsync(id);
        return delete;
    }
    private static BoardDto MapToDto(Board board) => new()
    {
        Id = board.Id,
        Title = board.Title,
        Description = board.Description,
        OwnerName = board.Owner.Username,
        CreatedAt = board.CreatedAt
    };
}
