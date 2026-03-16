using System.Net.Http.Json;
using Projekthantering.Shared.DTOs;

namespace Projekthantering.Client.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly LocalStorageService _localStorage;
    private readonly AuthStateProvider _authStateProvider;

    public AuthService(HttpClient http, LocalStorageService localStorage,
        AuthStateProvider authStateProvider)
    {
        _http = http;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", request);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (result != null)
        {
            await _localStorage.SetItemAsync("authToken", result.Token);
            _authStateProvider.NotifyAuthStateChanged();
        }
        return result;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", request);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (result != null)
        {
            await _localStorage.SetItemAsync("authToken", result.Token);
            _authStateProvider.NotifyAuthStateChanged();
        }
        return result;
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");
        _authStateProvider.NotifyAuthStateChanged();
    }
}
