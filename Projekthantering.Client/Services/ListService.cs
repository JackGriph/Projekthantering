using System.Net.Http.Json;
using Projekthantering.Shared.DTOs;

namespace Projekthantering.Client.Services;

public class ListService : IListService
{
    private readonly HttpClient _http;

    public ListService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<BoardListDto>> GetListsAsync(int boardId)
    {
        var result = await _http.GetFromJsonAsync<List<BoardListDto>>(
            $"api/boards/{boardId}/lists");
        return result ?? [];
    }

    public async Task<BoardListDto?> CreateListAsync(int boardId, CreateListRequest request)
    {
        var response = await _http.PostAsJsonAsync(
            $"api/boards/{boardId}/lists", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<BoardListDto>();
    }

    public async Task<BoardListDto?> UpdateListAsync(int id, UpdateListRequest request)
    {
        var response = await _http.PatchAsJsonAsync($"api/lists/{id}", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<BoardListDto>();
    }

    public async Task<bool> DeleteListAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/lists/{id}");
        return response.IsSuccessStatusCode;
    }
}
