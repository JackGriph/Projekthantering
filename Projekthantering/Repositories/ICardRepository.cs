using Projekthantering.Models;

namespace Projekthantering.Repositories;

public interface ICardRepository
{
    Task<List<Card>> GetByListIdAsync(int listId);
    Task<Card?> GetByIdAsync(int id);
    Task<Card> CreateAsync(Card card);
    Task<Card> UpdateAsync(Card card);
    Task<bool> DeleteAsync(int id);
    Task<bool> ListExistsAsync(int listId);
}
