using Projekthantering.Shared.DTOs;

namespace Projekthantering.Client.Services;

public interface IListService
{
    Task<List<BoardListDto>> GetListsAsync(int boardId);
    Task<BoardListDto?> CreateListAsync(int boardId, CreateListRequest request);
    Task<BoardListDto?> UpdateListAsync(int id, UpdateListRequest request);
    Task<bool> DeleteListAsync(int id);
}
