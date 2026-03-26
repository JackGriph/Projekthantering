using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Projekthantering.Client.Services;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedLocalStorage _localStorage;
    private readonly TokenProvider _tokenProvider;
    private readonly AuthenticationState _anonymous;

    public AuthStateProvider(ProtectedLocalStorage localStorage, TokenProvider tokenProvider)
    {
        _localStorage = localStorage;
        _tokenProvider = tokenProvider;
        _anonymous = new AuthenticationState(
            new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Först: kolla in-memory token
        var token = _tokenProvider.AccessToken;

        // Om tomt: försök hämta från ProtectedLocalStorage (vid sidladdning)
        if (string.IsNullOrWhiteSpace(token))
        {
            try
            {
                var result = await _localStorage.GetAsync<string>("accessToken");
                token = result.Value;

                // Synka tillbaka till TokenProvider så att AuthHeaderHandler kan använda den
                if (!string.IsNullOrWhiteSpace(token))
                {
                    _tokenProvider.AccessToken = token;

                    var refreshResult = await _localStorage.GetAsync<string>("refreshToken");
                    _tokenProvider.RefreshToken = refreshResult.Value;
                }
            }
            catch (InvalidOperationException)
            {
                // JS interop inte tillgängligt under prerendering
                return _anonymous;
            }
        }

        if (string.IsNullOrWhiteSpace(token))
            return _anonymous;

        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void NotifyUserAuthentication(string token)
    {
        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var authState = Task.FromResult(
            new AuthenticationState(new ClaimsPrincipal(identity)));
        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer
            .Deserialize<Dictionary<string, object>>(jsonBytes);
        return keyValuePairs!.Select(kvp =>
            new Claim(kvp.Key, kvp.Value.ToString()!));
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        base64 = base64.Replace('-', '+').Replace('_', '/');
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
