using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Projekthantering.Shared.DTOs;

namespace Projekthantering.Client.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;
    private readonly ProtectedLocalStorage _localStorage;
    private readonly AuthStateProvider _authStateProvider;

    public AuthService(HttpClient http, ProtectedLocalStorage localStorage,
        AuthStateProvider authStateProvider)
    {
        _http = http;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return AuthResult.Fail(error);
        }

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (authResponse == null)
            return AuthResult.Fail("Ogiltigt svar från servern.");

        await _localStorage.SetAsync("accessToken", authResponse.AccessToken);
        await _localStorage.SetAsync("refreshToken", authResponse.RefreshToken);
        _authStateProvider.NotifyUserAuthentication(authResponse.AccessToken);

        return AuthResult.Ok();
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return AuthResult.Fail(error);
        }

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (authResponse == null)
            return AuthResult.Fail("Ogiltigt svar från servern.");

        await _localStorage.SetAsync("accessToken", authResponse.AccessToken);
        await _localStorage.SetAsync("refreshToken", authResponse.RefreshToken);
        _authStateProvider.NotifyUserAuthentication(authResponse.AccessToken);

        return AuthResult.Ok();
    }

    public async Task LogoutAsync()
    {
        await _localStorage.DeleteAsync("accessToken");
        await _localStorage.DeleteAsync("refreshToken");
        _authStateProvider.NotifyUserLogout();
    }

    public async Task<bool> RefreshTokenAsync()
    {
        try
        {
            var result = await _localStorage.GetAsync<string>("refreshToken");
            var refreshToken = result.Value;

            if (string.IsNullOrEmpty(refreshToken))
                return false;

            var response = await _http.PostAsJsonAsync("api/auth/refresh",
                new { RefreshToken = refreshToken });

            if (!response.IsSuccessStatusCode)
                return false;

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (authResponse == null)
                return false;

            await _localStorage.SetAsync("accessToken", authResponse.AccessToken);
            await _localStorage.SetAsync("refreshToken", authResponse.RefreshToken);
            _authStateProvider.NotifyUserAuthentication(authResponse.AccessToken);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
