using Microsoft.EntityFrameworkCore;
using Projekthantering.Data;
using Projekthantering.Models;

namespace Projekthantering.Repositories;

public class ListRepository : IListRepository
{
    private readonly AppDbContext _context;

    public ListRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<BoardList>> GetByBoardIdAsync(int boardId)
        => await _context.BoardLists
            .Where(l => l.BoardId == boardId)
            .OrderBy(l => l.Position)
            .Include(l => l.Cards)
                .ThenInclude(c => c.Assignee)
            .ToListAsync();

    public async Task<BoardList?> GetByIdAsync(int id)
        => await _context.BoardLists
            .Include(l => l.Cards)
                .ThenInclude(c => c.Assignee)
            .FirstOrDefaultAsync(l => l.Id == id);

    public async Task<BoardList> CreateAsync(BoardList list)
    {
        _context.BoardLists.Add(list);
        await _context.SaveChangesAsync();
        return list;
    }

    public async Task<BoardList> UpdateAsync(BoardList list)
    {
        _context.BoardLists.Update(list);
        await _context.SaveChangesAsync();
        return list;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var list = await _context.BoardLists.FindAsync(id);
        if (list is null) return false;

        _context.BoardLists.Remove(list);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> BoardExistsAsync(int boardId)
        => await _context.Boards.AnyAsync(b => b.Id == boardId);
}
