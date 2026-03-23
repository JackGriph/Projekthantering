using Microsoft.EntityFrameworkCore;
using Projekthantering.Data;
using Projekthantering.Models;

namespace Projekthantering.Repositories;

public class BoardRepository : IBoardRepository
{
    private readonly AppDbContext _context;

    public BoardRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Board>> GetAllAsync()
        => await _context.Boards
            .Include(b => b.Owner)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

    public async Task<List<Board>> GetByUserIdAsync(int userId)
        => await _context.Boards
            .Where(b => b.OwnerId == userId || b.Members.Any(m => m.UserId == userId))
            .Include(b => b.Owner)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

    public async Task<Board?> GetByIdAsync(int id)
        => await _context.Boards
            .Include(b => b.Owner)
            .Include(b => b.Lists)
                .ThenInclude(l => l.Cards)
            .Include(b => b.Members)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(b => b.Id == id);

    public async Task<Board> CreateAsync(Board board)
    {
        _context.Boards.Add(board);
        await _context.SaveChangesAsync();
        await _context.Entry(board).Reference(b => b.Owner).LoadAsync();
        return board;
    }

    public async Task<Board> UpdateAsync(Board board)
    {
        _context.Boards.Update(board);
        await _context.SaveChangesAsync();
        return board;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var board = await _context.Boards.FindAsync(id);
        if (board is null) return false;
        _context.Boards.Remove(board);
        await _context.SaveChangesAsync();
        return true;
    }
}
