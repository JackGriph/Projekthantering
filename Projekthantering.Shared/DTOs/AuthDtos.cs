using System.ComponentModel.DataAnnotations;

namespace Projekthantering.Shared.DTOs;

public class RegisterRequest
{
    [Required(ErrorMessage = "Användarnamn krävs.")]
    [MinLength(3, ErrorMessage = "Användarnamn måste vara minst 3 tecken.")]
    [MaxLength(50, ErrorMessage = "Användarnamn får vara max 50 tecken.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email krävs.")]
    [EmailAddress(ErrorMessage = "Ogiltig emailadress.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lösenord krävs.")]
    [MinLength(6, ErrorMessage = "Lösenord måste vara minst 6 tecken.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Bekräfta lösenord krävs.")]
    [Compare("Password", ErrorMessage = "Lösenorden matchar inte.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class LoginRequest
{
    [Required(ErrorMessage = "Email krävs.")]
    [EmailAddress(ErrorMessage = "Ogiltig emailadress.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lösenord krävs.")]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class RefreshRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}

public record AuthResult(bool Success, string? ErrorMessage = null)
{
    public static AuthResult Ok() => new(true);
    public static AuthResult Fail(string message) => new(false, message);
}
