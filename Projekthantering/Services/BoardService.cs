using Projekthantering.Models;
using Projekthantering.Repositories;
using Projekthantering.Shared.DTOs;

namespace Projekthantering.Services;

public class BoardService : IBoardService
{
    private readonly IBoardRepository _boardRepository;
    private readonly IUserRepository _userRepository;

    public BoardService(IBoardRepository boardRepository, IUserRepository userRepository)
    {
        _boardRepository = boardRepository;
        _userRepository  = userRepository;
    }

    public async Task<List<BoardDto>> GetBoardsAsync()
    {
        var boards = await _boardRepository.GetAllAsync();
        return boards.Select(MapToDto).ToList();
    }

    public async Task<BoardDto?> GetBoardByIdAsync(int id)
    {
        var board = await _boardRepository.GetByIdAsync(id);
        return board is null ? null : MapToDto(board);
    }

    public async Task<BoardDto> CreateBoardAsync(CreateBoardRequest request, int ownerId)
    {
        var board = new Board
        {
            Title       = request.Title,
            Description = request.Description,
            OwnerId     = ownerId
        };

        var created = await _boardRepository.CreateAsync(board);
        return MapToDto(created);
    }

    public async Task<BoardDto?> UpdateBoardAsync(int id, UpdateBoardRequest request)
    {
        var board = await _boardRepository.GetByIdAsync(id);
        if (board is null) return null;

        board.Title       = request.Title;
        board.Description = request.Description;

        var updated = await _boardRepository.UpdateAsync(board);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteBoardAsync(int id)
        => await _boardRepository.DeleteAsync(id);

    private static BoardDto MapToDto(Board board) => new()
    {
        Id          = board.Id,
        Title       = board.Title,
        Description = board.Description,
        OwnerName   = board.Owner.Username,
        CreatedAt   = board.CreatedAt
    };
}
