using Microsoft.EntityFrameworkCore;
using Projekthantering.Data;
using Projekthantering.Models;

namespace Projekthantering.Repositories;

public class CardRepository : ICardRepository
{
    private readonly AppDbContext _context;

    public CardRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Card>> GetByListIdAsync(int listId)
        => await _context.Cards
            .Where(c => c.ListId == listId)
            .OrderBy(c => c.Position)
            .Include(c => c.Assignee)
            .ToListAsync();

    public async Task<Card?> GetByIdAsync(int id)
        => await _context.Cards
            .Include(c => c.Assignee)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Card> CreateAsync(Card card)
    {
        _context.Cards.Add(card);
        await _context.SaveChangesAsync();
        return card;
    }

    public async Task<Card> UpdateAsync(Card card)
    {
        _context.Cards.Update(card);
        await _context.SaveChangesAsync();
        return card;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var card = await _context.Cards.FindAsync(id);
        if (card is null) return false;

        _context.Cards.Remove(card);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ListExistsAsync(int listId)
        => await _context.BoardLists.AnyAsync(l => l.Id == listId);
}
