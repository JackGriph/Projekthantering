using Projekthantering.Shared.DTOs;

namespace Projekthantering.Services;

public interface IBoardService
{
    Task<List<BoardDto>> GetBoardsAsync();
    Task<BoardDto?> GetBoardByIdAsync(int id);
    Task<BoardDto> CreateBoardAsync(CreateBoardRequest request, int ownerId);
    Task<BoardDto?> UpdateBoardAsync(int id, UpdateBoardRequest request);
    Task<bool> DeleteBoardAsync(int id);
}
