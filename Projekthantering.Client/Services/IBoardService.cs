using Projekthantering.Shared.DTOs;

namespace Projekthantering.Client.Services;

public interface IBoardService
{
    Task<List<BoardDto>> GetBoardsAsync();
    Task<BoardDto?> GetBoardAsync(int id);
    Task<BoardDto?> CreateBoardAsync(CreateBoardRequest request);
    Task<bool> DeleteBoardAsync(int id);
}
