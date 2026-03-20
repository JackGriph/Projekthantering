namespace Projekthantering.Client.Services;

/// <summary>
/// Håller JWT-token i minnet under den aktiva Blazor-sessionen (circuit).
/// Registreras som Scoped så att samma instans delas av AuthService,
/// AuthStateProvider och AuthHeaderHandler inom samma circuit.
/// ProtectedLocalStorage används fortfarande för persistens (sidladdning),
/// men alla runtime-läsningar av token görs härifrån.
/// </summary>
public class TokenProvider
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}
