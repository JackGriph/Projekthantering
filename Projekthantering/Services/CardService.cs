using Projekthantering.Models;
using Projekthantering.Repositories;
using Projekthantering.Shared.DTOs;

namespace Projekthantering.Services;

public class CardService : ICardService
{
    private readonly ICardRepository _cardRepository;
    private readonly IUserRepository _userRepository;

    public CardService(ICardRepository cardRepository, IUserRepository userRepository)
    {
        _cardRepository = cardRepository;
        _userRepository = userRepository;
    }

    public async Task<bool> ListExistsAsync(int listId)
        => await _cardRepository.ListExistsAsync(listId);

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
        card.DueDate = request.DueDate;

        if (!string.IsNullOrWhiteSpace(request.AssignedTo))
        {
            var user = await _userRepository.GetByUsernameAsync(request.AssignedTo.Trim());
            card.AssigneeId = user?.Id;
        }
        else if (request.AssignedTo is not null)
        {
            // empty string explicitly clears the assignee
            card.AssigneeId = null;
        }
        else
        {
            card.AssigneeId = request.AssigneeId;
        }

        var updated = await _cardRepository.UpdateAsync(card);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteCardAsync(int id)
        => await _cardRepository.DeleteAsync(id);

    public async Task<CardDto?> MoveCardAsync(int id, MoveCardRequest request)
    {
        var card = await _cardRepository.GetByIdAsync(id);
        if (card is null) return null;

        var targetListExists = await _cardRepository.ListExistsAsync(request.TargetListId);
        if (!targetListExists) return null;

        card.ListId = request.TargetListId;
        card.Position = request.Position;

        var updated = await _cardRepository.UpdateAsync(card);
        return MapToDto(updated);
    }

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
