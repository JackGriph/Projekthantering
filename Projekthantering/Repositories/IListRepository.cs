using Projekthantering.Models;

namespace Projekthantering.Repositories;

public interface IListRepository
{
    Task<List<BoardList>> GetByBoardIdAsync(int boardId);
    Task<BoardList?> GetByIdAsync(int id);
    Task<BoardList> CreateAsync(BoardList list);
    Task<BoardList> UpdateAsync(BoardList list);
    Task<bool> DeleteAsync(int id);
    Task<bool> BoardExistsAsync(int boardId);
}
