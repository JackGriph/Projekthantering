using Projekthantering.Shared.DTOs;

namespace Projekthantering.Client.Services;

public interface ICardService
{
    Task<List<CardDto>> GetCardsAsync(int listId);
    Task<CardDto?> CreateCardAsync(int listId, CreateCardRequest request);
    Task<CardDto?> UpdateCardAsync(int id, UpdateCardRequest request);
    Task<bool> DeleteCardAsync(int id);
    Task<CardDto?> MoveCardAsync(int id, MoveCardRequest request);
}
