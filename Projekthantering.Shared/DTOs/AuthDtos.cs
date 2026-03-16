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
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
