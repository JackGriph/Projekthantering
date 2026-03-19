using System.Net.Http.Json;
using Projekthantering.Shared.DTOs;

namespace Projekthantering.Client.Services;

public class BoardService : IBoardService
{
    private readonly HttpClient _http;

    public BoardService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<BoardDto>> GetBoardsAsync()
    {
        var result = await _http.GetFromJsonAsync<List<BoardDto>>("api/boards");
        return result ?? [];
    }

    public async Task<BoardDto?> GetBoardAsync(int id)
        => await _http.GetFromJsonAsync<BoardDto>($"api/boards/{id}");

    public async Task<BoardDto?> CreateBoardAsync(CreateBoardRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/boards", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<BoardDto>();
    }

    public async Task<bool> DeleteBoardAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/boards/{id}");
        return response.IsSuccessStatusCode;
    }
}
