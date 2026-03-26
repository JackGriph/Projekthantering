using Moq;
using Projekthantering.Models;
using Projekthantering.Repositories;
using Projekthantering.Services;
using Projekthantering.Shared.DTOs;

namespace Projekthantering.Tests;

public class BoardServiceTests
{
    private readonly Mock<IBoardRepository> _boardRepoMock = new();

    private BoardService CreateSut() =>
        new(_boardRepoMock.Object);

    private static Board MakeBoard(int id, int ownerId, string ownerName = "ägare") => new()
    {
        Id = id,
        Title = $"Tavla {id}",
        OwnerId = ownerId,
        Owner = new User { Id = ownerId, Username = ownerName },
        Members = new List<BoardMember>()
    };

    // Test 1
    // GetBoardByIdAsync ska returnera null om tavlan inte finns
    [Fact]
    public async Task GetBoardByIdAsync_WhenBoardDoesNotExist_ReturnsNull()
    {
        // Arrange
        _boardRepoMock
            .Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((Board?)null);

        var sut = CreateSut();

        // Act
        var result = await sut.GetBoardByIdAsync(id: 99, userId: 1);

        // Assert
        Assert.Null(result);
    }

    // Test 2
    // GetBoardByIdAsync ska returnera null om användaren varken är ägare eller medlem
    [Fact]
    public async Task GetBoardByIdAsync_WhenUserHasNoAccess_ReturnsNull()
    {
        // Arrange
        var board = MakeBoard(id: 1, ownerId: 10);

        _boardRepoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(board);

        var sut = CreateSut();

        // Act
        var result = await sut.GetBoardByIdAsync(id: 1, userId: 99); // varken ägare (10) eller medlem

        // Assert
        Assert.Null(result);
    }

    // Test 3
    // GetBoardByIdAsync ska returnera tavlan om användaren är ägare
    [Fact]
    public async Task GetBoardByIdAsync_WhenUserIsOwner_ReturnsBoardDto()
    {
        // Arrange
        var board = MakeBoard(id: 1, ownerId: 5, ownerName: "ägaren");

        _boardRepoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(board);

        var sut = CreateSut();

        // Act
        var result = await sut.GetBoardByIdAsync(id: 1, userId: 5);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Tavla 1", result.Title);
        Assert.Equal("ägaren", result.OwnerName);
    }

    // Test 4
    // GetBoardByIdAsync ska returnera tavlan om användaren är medlem
    [Fact]
    public async Task GetBoardByIdAsync_WhenUserIsMember_ReturnsBoardDto()
    {
        // Arrange
        var board = MakeBoard(id: 2, ownerId: 10);
        board.Members.Add(new BoardMember { UserId = 7, BoardId = 2 });

        _boardRepoMock
            .Setup(r => r.GetByIdAsync(2))
            .ReturnsAsync(board);

        var sut = CreateSut();

        // Act
        var result = await sut.GetBoardByIdAsync(id: 2, userId: 7);

        // Assert
        Assert.NotNull(result);
    }

    // Test 5
    // UpdateBoardAsync ska returnera null om användaren inte är ägare
    [Fact]
    public async Task UpdateBoardAsync_WhenUserIsNotOwner_ReturnsNull()
    {
        // Arrange
        var board = MakeBoard(id: 1, ownerId: 10);

        _boardRepoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(board);

        var sut = CreateSut();
        var request = new UpdateBoardRequest { Title = "Ny titel" };

        // Act
        var result = await sut.UpdateBoardAsync(id: 1, request, userId: 99);

        // Assert
        Assert.Null(result);
    }

    // Test 6
    // DeleteBoardAsync ska returnera false om användaren inte är ägare
    [Fact]
    public async Task DeleteBoardAsync_WhenUserIsNotOwner_ReturnsFalse()
    {
        // Arrange
        var board = MakeBoard(id: 1, ownerId: 10);

        _boardRepoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(board);

        var sut = CreateSut();

        // Act
        var result = await sut.DeleteBoardAsync(id: 1, userId: 99);

        // Assert
        Assert.False(result);
    }

    // Test 7
    // CreateBoardAsync ska sätta korrekt ägare och titel
    [Fact]
    public async Task CreateBoardAsync_WhenCalled_ReturnsBoardDtoWithCorrectOwnerAndTitle()
    {
        // Arrange
        _boardRepoMock
            .Setup(r => r.CreateAsync(It.IsAny<Board>()))
            .ReturnsAsync((Board b) =>
            {
                b.Owner = new User { Id = b.OwnerId, Username = "skaparen" };
                return b;
            });

        var sut = CreateSut();
        var request = new CreateBoardRequest { Title = "Min tavla", Description = "En beskrivning" };

        // Act
        var result = await sut.CreateBoardAsync(request, ownerId: 3);

        // Assert
        Assert.Equal("Min tavla", result.Title);
        Assert.Equal("skaparen", result.OwnerName);
    }
}
