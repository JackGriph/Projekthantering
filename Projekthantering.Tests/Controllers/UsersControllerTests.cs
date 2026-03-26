using Microsoft.AspNetCore.Mvc;
using Moq;
using Projekthantering.Controllers;
using Projekthantering.Models;
using Projekthantering.Repositories;
using Projekthantering.Shared.DTOs;

namespace Projekthantering.Tests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();

    private UsersController CreateSut() =>
        new(_userRepoMock.Object);

    // Test 1
    // GetAll ska returnera OkObjectResult med lista av UserDto
    [Fact]
    public async Task GetAll_WhenCalled_ReturnsOkWithUserDtos()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Id = 1, Username = "alice", Email = "alice@example.com", PasswordHash = "hash" },
            new() { Id = 2, Username = "bob",   Email = "bob@example.com",   PasswordHash = "hash" }
        };

        _userRepoMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(users);

        var sut = CreateSut();

        // Act
        var result = await sut.GetAll();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var dtos = Assert.IsType<List<UserDto>>(ok.Value);
        Assert.Equal(2, dtos.Count);
        Assert.Equal("alice", dtos[0].Username);
        Assert.Equal("bob", dtos[1].Username);
    }
}
