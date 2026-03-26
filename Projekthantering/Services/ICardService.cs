using Projekthantering.Shared.DTOs;

namespace Projekthantering.Services;

public interface ICardService
{
    Task<bool> ListExistsAsync(int listId);
    Task<List<CardDto>> GetCardsByListAsync(int listId);
    Task<CardDto?> GetCardByIdAsync(int id);
    Task<CardDto> CreateCardAsync(int listId, CreateCardRequest request);
    Task<CardDto?> UpdateCardAsync(int id, UpdateCardRequest request);
    Task<bool> DeleteCardAsync(int id);
    Task<CardDto?> MoveCardAsync(int id, MoveCardRequest request);
}
