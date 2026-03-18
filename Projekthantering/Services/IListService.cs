using Projekthantering.Shared.DTOs;

namespace Projekthantering.Services;

public interface IListService
{
    Task<List<BoardListDto>> GetListsByBoardAsync(int boardId);
    Task<BoardListDto?> GetListByIdAsync(int id);
    Task<BoardListDto> CreateListAsync(int boardId, CreateListRequest request);
    Task<BoardListDto?> UpdateListAsync(int id, UpdateListRequest request);
    Task<bool> DeleteListAsync(int id);
}
