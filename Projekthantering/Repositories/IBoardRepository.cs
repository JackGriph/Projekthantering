using Projekthantering.Models;

namespace Projekthantering.Repositories;

public interface IBoardRepository
{
    Task<List<Board>> GetAllAsync();
    Task<List<Board>> GetByUserIdAsync(int userId);
    Task<Board?> GetByIdAsync(int id);
    Task<Board> CreateAsync(Board board);
    Task<Board> UpdateAsync(Board board);
    Task<bool> DeleteAsync(int id);
}
