using Microsoft.AspNetCore.Mvc;
using Moq;
using Projekthantering.Controllers;
using Projekthantering.Services;
using Projekthantering.Shared.DTOs;

namespace Projekthantering.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock = new();

    private AuthController CreateSut() =>
        new(_authServiceMock.Object);

    // Test 1
    // Register ska returnera OkObjectResult med AuthResponse vid lyckad registrering
    [Fact]
    public async Task Register_WhenSuccessful_ReturnsOkWithAuthResponse()
    {
        // Arrange
        var authResponse = new AuthResponse { Username = "newuser", AccessToken = "token" };
        _authServiceMock
            .Setup(s => s.RegisterAsync(It.IsAny<RegisterRequest>()))
            .ReturnsAsync(authResponse);

        var sut = CreateSut();
        var request = new RegisterRequest
        {
            Username = "newuser",
            Email = "new@example.com",
            Password = "password123",
            ConfirmPassword = "password123"
        };

        // Act
        var result = await sut.Register(request);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<AuthResponse>(ok.Value);
        Assert.Equal("newuser", body.Username);
    }

    // Test 2
    // Register ska returnera BadRequest när emailen redan finns
    [Fact]
    public async Task Register_WhenEmailTaken_ReturnsBadRequest()
    {
        // Arrange
        _authServiceMock
            .Setup(s => s.RegisterAsync(It.IsAny<RegisterRequest>()))
            .ThrowsAsync(new InvalidOperationException("Email redan registrerad."));

        var sut = CreateSut();

        // Act
        var result = await sut.Register(new RegisterRequest
        {
            Username = "user",
            Email = "taken@example.com",
            Password = "password123",
            ConfirmPassword = "password123"
        });

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    // Test 3
    // Login ska returnera OkObjectResult med AuthResponse vid korrekt inloggning
    [Fact]
    public async Task Login_WhenSuccessful_ReturnsOkWithAuthResponse()
    {
        // Arrange
        var authResponse = new AuthResponse { Username = "user", AccessToken = "token" };
        _authServiceMock
            .Setup(s => s.LoginAsync(It.IsAny<LoginRequest>()))
            .ReturnsAsync(authResponse);

        var sut = CreateSut();

        // Act
        var result = await sut.Login(new LoginRequest
        {
            Email = "user@example.com",
            Password = "password123"
        });

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<AuthResponse>(ok.Value);
        Assert.Equal("user", body.Username);
    }

    // Test 4
    // Login ska returnera BadRequest vid felaktigt lösenord
    [Fact]
    public async Task Login_WhenPasswordWrong_ReturnsBadRequest()
    {
        // Arrange
        _authServiceMock
            .Setup(s => s.LoginAsync(It.IsAny<LoginRequest>()))
            .ThrowsAsync(new InvalidOperationException("Felaktiga inloggningsuppgifter."));

        var sut = CreateSut();

        // Act
        var result = await sut.Login(new LoginRequest
        {
            Email = "user@example.com",
            Password = "wrongpassword"
        });

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    // Test 5
    // Refresh ska returnera OkObjectResult med AuthResponse vid giltig refresh token
    [Fact]
    public async Task Refresh_WhenSuccessful_ReturnsOkWithAuthResponse()
    {
        // Arrange
        var authResponse = new AuthResponse { Username = "user", AccessToken = "new-token", RefreshToken = "new-refresh" };
        _authServiceMock
            .Setup(s => s.RefreshTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(authResponse);

        var sut = CreateSut();

        // Act
        var result = await sut.Refresh(new RefreshRequest { RefreshToken = "valid-refresh-token" });

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<AuthResponse>(ok.Value);
        Assert.Equal("user", body.Username);
    }

    // Test 6
    // Refresh ska returnera BadRequest vid ogiltig refresh token
    [Fact]
    public async Task Refresh_WhenTokenInvalid_ReturnsBadRequest()
    {
        // Arrange
        _authServiceMock
            .Setup(s => s.RefreshTokenAsync(It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("Ogiltig refresh token."));

        var sut = CreateSut();

        // Act
        var result = await sut.Refresh(new RefreshRequest { RefreshToken = "bad-token" });

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}
