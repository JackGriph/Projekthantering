using Projekthantering.Models;
using Projekthantering.Repositories;
using Projekthantering.Shared.DTOs;

namespace Projekthantering.Services;

public class CardService : ICardService
{
    private readonly ICardRepository _cardRepository;

    public CardService(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public async Task<List<CardDto>> GetCardsByListAsync(int listId)
    {
        var cards = await _cardRepository.GetByListIdAsync(listId);
        return cards.Select(MapToDto).ToList();
    }

    public async Task<CardDto?> GetCardByIdAsync(int id)
    {
        var card = await _cardRepository.GetByIdAsync(id);
        return card is null ? null : MapToDto(card);
    }

    public async Task<CardDto> CreateCardAsync(int listId, CreateCardRequest request)
    {
        var existingCards = await _cardRepository.GetByListIdAsync(listId);
        var nextPosition = existingCards.Count > 0
            ? existingCards.Max(c => c.Position) + 1
            : 0;

        var status = CardStatus.IsValid(request.Status) ? request.Status : CardStatus.Todo;

        var card = new Card
        {
            Title = request.Title,
            Description = request.Description,
            ListId = listId,
            Position = nextPosition,
            Status = status,
            AssigneeId = request.AssigneeId
        };

        var created = await _cardRepository.CreateAsync(card);
        return MapToDto(created);
    }

    public async Task<CardDto?> UpdateCardAsync(int id, UpdateCardRequest request)
    {
        var card = await _cardRepository.GetByIdAsync(id);
        if (card is null) return null;

        card.Title = request.Title;
        card.Description = request.Description;
        card.Status = CardStatus.IsValid(request.Status) ? request.Status : card.Status;
        card.AssigneeId = request.AssigneeId;
        card.DueDate = request.DueDate;

        var updated = await _cardRepository.UpdateAsync(card);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteCardAsync(int id)
        => await _cardRepository.DeleteAsync(id);

    private static CardDto MapToDto(Card card) => new()
    {
        Id = card.Id,
        Title = card.Title,
        Description = card.Description,
        ListId = card.ListId,
        Position = card.Position,
        Status = card.Status,
        AssigneeId = card.AssigneeId,
        AssigneeName = card.Assignee?.Username,
        DueDate = card.DueDate,
        CreatedAt = card.CreatedAt
    };
}
