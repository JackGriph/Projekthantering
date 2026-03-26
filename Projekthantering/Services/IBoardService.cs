using Projekthantering.Shared.DTOs;

namespace Projekthantering.Services;

public interface IBoardService
{
    Task<List<BoardDto>> GetBoardsAsync();
    Task<List<BoardDto>> GetBoardsByUserAsync(int userId);
    Task<BoardDto?> GetBoardByIdAsync(int id, int userId);
    Task<BoardDto> CreateBoardAsync(CreateBoardRequest request, int ownerId);
    Task<BoardDto?> UpdateBoardAsync(int id, UpdateBoardRequest request, int userId);
    Task<bool> DeleteBoardAsync(int id, int userId);
}
