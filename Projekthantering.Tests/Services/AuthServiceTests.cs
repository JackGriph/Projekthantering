using Microsoft.Extensions.Configuration;
using Moq;
using Projekthantering.Models;
using Projekthantering.Repositories;
using Projekthantering.Services;
using Projekthantering.Shared.DTOs;

namespace Projekthantering.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();

    private AuthService CreateSut()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "super-secret-test-key-that-is-long-enough-for-hmac",
                ["Jwt:Issuer"] = "test-issuer",
                ["Jwt:Audience"] = "test-audience"
            })
            .Build();

        return new AuthService(_userRepoMock.Object, config);
    }

    // Test 1
    // RegisterAsync ska kasta undantag om emailen redan är registrerad
    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ThrowsInvalidOperationException()
    {
        // Arrange
        _userRepoMock
            .Setup(r => r.EmailExistsAsync("existing@example.com"))
            .ReturnsAsync(true);

        var sut = CreateSut();
        var request = new RegisterRequest
        {
            Username = "newuser",
            Email = "existing@example.com",
            Password = "password123",
            ConfirmPassword = "password123"
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.RegisterAsync(request));

        Assert.Equal("Email redan registrerad.", ex.Message);
    }

    // Test 2
    // RegisterAsync ska kasta undantag om användarnamnet redan är taget
    [Fact]
    public async Task RegisterAsync_WhenUsernameAlreadyExists_ThrowsInvalidOperationException()
    {
        // Arrange
        _userRepoMock
            .Setup(r => r.EmailExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _userRepoMock
            .Setup(r => r.UsernameExistsAsync("takenuser"))
            .ReturnsAsync(true);

        var sut = CreateSut();
        var request = new RegisterRequest
        {
            Username = "takenuser",
            Email = "new@example.com",
            Password = "password123",
            ConfirmPassword = "password123"
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.RegisterAsync(request));

        Assert.Equal("Användarnamn redan taget.", ex.Message);
    }

    // Test 3
    // RegisterAsync ska returnera AuthResponse med korrekt användarnamn vid lyckad registrering
    [Fact]
    public async Task RegisterAsync_WhenValidRequest_ReturnsAuthResponseWithUsername()
    {
        // Arrange
        _userRepoMock
            .Setup(r => r.EmailExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _userRepoMock
            .Setup(r => r.UsernameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _userRepoMock
            .Setup(r => r.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        _userRepoMock
            .Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        var sut = CreateSut();
        var request = new RegisterRequest
        {
            Username = "newuser",
            Email = "new@example.com",
            Password = "password123",
            ConfirmPassword = "password123"
        };

        // Act
        var result = await sut.RegisterAsync(request);

        // Assert
        Assert.Equal("newuser", result.Username);
        Assert.False(string.IsNullOrEmpty(result.AccessToken));
        Assert.False(string.IsNullOrEmpty(result.RefreshToken));
    }

    // Test 4
    // LoginAsync ska kasta undantag om emailen inte finns
    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        _userRepoMock
            .Setup(r => r.GetByEmailAsync("unknown@example.com"))
            .ReturnsAsync((User?)null);

        var sut = CreateSut();
        var request = new LoginRequest
        {
            Email = "unknown@example.com",
            Password = "anypassword"
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.LoginAsync(request));

        Assert.Equal("Felaktiga inloggningsuppgifter.", ex.Message);
    }

    // Test 5
    // LoginAsync ska kasta undantag om lösenordet är felaktigt
    [Fact]
    public async Task LoginAsync_WhenPasswordIsWrong_ThrowsInvalidOperationException()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
            Role = "User"
        };

        _userRepoMock
            .Setup(r => r.GetByEmailAsync("test@example.com"))
            .ReturnsAsync(user);

        var sut = CreateSut();
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "wrongpassword"
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.LoginAsync(request));

        Assert.Equal("Felaktiga inloggningsuppgifter.", ex.Message);
    }

    // Test 6
    // RefreshTokenAsync ska returnera AuthResponse med ny token vid giltig refresh token
    [Fact]
    public async Task RefreshTokenAsync_WhenTokenIsValid_ReturnsAuthResponse()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash",
            Role = "User",
            RefreshToken = "valid-refresh-token",
            RefreshTokenExpiry = DateTime.UtcNow.AddDays(7)
        };

        _userRepoMock
            .Setup(r => r.GetByRefreshTokenAsync("valid-refresh-token"))
            .ReturnsAsync(user);

        _userRepoMock
            .Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        var sut = CreateSut();

        // Act
        var result = await sut.RefreshTokenAsync("valid-refresh-token");

        // Assert
        Assert.Equal("testuser", result.Username);
        Assert.False(string.IsNullOrEmpty(result.AccessToken));
        Assert.False(string.IsNullOrEmpty(result.RefreshToken));
    }

    // Test 7
    // RefreshTokenAsync ska kasta undantag om refresh token inte finns
    [Fact]
    public async Task RefreshTokenAsync_WhenTokenIsInvalid_ThrowsInvalidOperationException()
    {
        // Arrange
        _userRepoMock
            .Setup(r => r.GetByRefreshTokenAsync("invalid-token"))
            .ReturnsAsync((User?)null);

        var sut = CreateSut();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.RefreshTokenAsync("invalid-token"));

        Assert.Equal("Ogiltig refresh token.", ex.Message);
    }

    // Test 8
    // RefreshTokenAsync ska kasta undantag om refresh token har gått ut
    [Fact]
    public async Task RefreshTokenAsync_WhenTokenIsExpired_ThrowsInvalidOperationException()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash",
            Role = "User",
            RefreshToken = "expired-token",
            RefreshTokenExpiry = DateTime.UtcNow.AddDays(-1)
        };

        _userRepoMock
            .Setup(r => r.GetByRefreshTokenAsync("expired-token"))
            .ReturnsAsync(user);

        var sut = CreateSut();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.RefreshTokenAsync("expired-token"));

        Assert.Equal("Refresh token har gått ut.", ex.Message);
    }
}