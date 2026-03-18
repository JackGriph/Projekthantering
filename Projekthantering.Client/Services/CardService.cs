using System.Net.Http.Json;
using Projekthantering.Shared.DTOs;

namespace Projekthantering.Client.Services;

public class CardService
{
    private readonly HttpClient _http;

    public CardService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<CardDto>> GetCardsAsync(int listId)
    {
        var result = await _http.GetFromJsonAsync<List<CardDto>>(
            $"api/lists/{listId}/cards");
        return result ?? [];
    }

    public async Task<CardDto?> CreateCardAsync(int listId, CreateCardRequest request)
    {
        var response = await _http.PostAsJsonAsync($"api/lists/{listId}/cards", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<CardDto>();
    }

    public async Task<CardDto?> UpdateCardAsync(int id, UpdateCardRequest request)
    {
        var response = await _http.PatchAsJsonAsync($"api/cards/{id}", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<CardDto>();
    }

    public async Task<bool> DeleteCardAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/cards/{id}");
        return response.IsSuccessStatusCode;
    }
}
