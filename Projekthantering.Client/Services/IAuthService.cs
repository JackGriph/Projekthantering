using Projekthantering.Shared.DTOs;

namespace Projekthantering.Client.Services;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    Task LogoutAsync();
    Task<bool> RefreshTokenAsync();
}
