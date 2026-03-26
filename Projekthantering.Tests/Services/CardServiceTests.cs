using Moq;
using Projekthantering.Models;
using Projekthantering.Repositories;
using Projekthantering.Services;
using Projekthantering.Shared.DTOs;

namespace Projekthantering.Tests;

public class CardServiceTests
{
    private readonly Mock<ICardRepository> _cardRepoMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();

    private CardService CreateSut() =>
        new(_cardRepoMock.Object, _userRepoMock.Object);

    //  Test 1 
    // CreateCardAsync ska tilldela position max+1 n�r listan redan har kort
    [Fact]
    public async Task CreateCardAsync_WhenListHasExistingCards_AssignsNextPosition()
    {
        // Arrange
        var existingCards = new List<Card>
        {
            new() { Id = 1, ListId = 10, Position = 0, Title = "Kort A" },
            new() { Id = 2, ListId = 10, Position = 1, Title = "Kort B" }
        };

        _cardRepoMock
            .Setup(r => r.GetByListIdAsync(10))
            .ReturnsAsync(existingCards);

        _cardRepoMock
            .Setup(r => r.CreateAsync(It.IsAny<Card>()))
            .ReturnsAsync((Card c) => c);

        var sut = CreateSut();
        var request = new CreateCardRequest { Title = "Nytt kort", Status = "todo" };

        // Act
        var result = await sut.CreateCardAsync(listId: 10, request);

        // Assert
        Assert.Equal(2, result.Position); // max(0,1) + 1 = 2
    }

    //  Test 2
    // CreateListAsync ska s�tta position 0 n�r tavlan inte har n�gra listor
    [Fact]
    public async Task CreateListAsync_WhenBoardHasNoLists_SetsPositionToZero()
    {
        // Arrange
        _cardRepoMock.Setup(r => r.GetByListIdAsync(It.IsAny<int>()))
            .ReturnsAsync([]);

        var listRepoMock = new Mock<IListRepository>();

        listRepoMock
            .Setup(r => r.GetByBoardIdAsync(5))
            .ReturnsAsync([]);

        listRepoMock
            .Setup(r => r.CreateAsync(It.IsAny<BoardList>()))
            .ReturnsAsync((BoardList l) => l);

        var sut = new ListService(listRepoMock.Object);
        var request = new CreateListRequest { Title = "Ny lista" };

        // Act
        var result = await sut.CreateListAsync(boardId: 5, request);

        // Assert
        Assert.Equal(0, result.Position);
    }

    //  Test 3
    // UpdateCardAsync ska returnera null om kortet inte finns
    [Fact]
    public async Task UpdateCardAsync_WhenCardDoesNotExist_ReturnsNull()
    {
        // Arrange
        _cardRepoMock
            .Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((Card?)null);

        var sut = CreateSut();
        var request = new UpdateCardRequest { Title = "Uppdaterad titel" };

        // Act
        var result = await sut.UpdateCardAsync(id: 99, request);

        // Assert
        Assert.Null(result);
    }

    // Test 4
    // CreateCardAsync ska sätta position 0 när listan är tom
    [Fact]
    public async Task CreateCardAsync_WhenListIsEmpty_SetsPositionToZero()
    {
        // Arrange
        _cardRepoMock
            .Setup(r => r.GetByListIdAsync(5))
            .ReturnsAsync([]);

        _cardRepoMock
            .Setup(r => r.CreateAsync(It.IsAny<Card>()))
            .ReturnsAsync((Card c) => c);

        var sut = CreateSut();
        var request = new CreateCardRequest { Title = "Första kortet", Status = "todo" };

        // Act
        var result = await sut.CreateCardAsync(listId: 5, request);

        // Assert
        Assert.Equal(0, result.Position);
    }

    // Test 5
    // GetCardByIdAsync ska returnera null om kortet inte finns
    [Fact]
    public async Task GetCardByIdAsync_WhenCardDoesNotExist_ReturnsNull()
    {
        // Arrange
        _cardRepoMock
            .Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((Card?)null);

        var sut = CreateSut();

        // Act
        var result = await sut.GetCardByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    // Test 6
    // GetCardByIdAsync ska returnera CardDto om kortet finns
    [Fact]
    public async Task GetCardByIdAsync_WhenCardExists_ReturnsCardDto()
    {
        // Arrange
        var card = new Card { Id = 1, Title = "Mitt kort", ListId = 3, Position = 0, Status = "todo" };

        _cardRepoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(card);

        var sut = CreateSut();

        // Act
        var result = await sut.GetCardByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Mitt kort", result.Title);
        Assert.Equal(1, result.Id);
    }

    // Test 7
    // UpdateCardAsync ska rensa tilldelning när AssignedTo är tom sträng
    [Fact]
    public async Task UpdateCardAsync_WhenAssignedToIsEmptyString_ClearsAssignee()
    {
        // Arrange
        var card = new Card { Id = 1, Title = "Kort", ListId = 2, AssigneeId = 5 };

        _cardRepoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(card);

        _cardRepoMock
            .Setup(r => r.UpdateAsync(It.IsAny<Card>()))
            .ReturnsAsync((Card c) => c);

        var sut = CreateSut();
        var request = new UpdateCardRequest { Title = "Kort", AssignedTo = "" }; // tom sträng = rensa

        // Act
        var result = await sut.UpdateCardAsync(id: 1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.AssigneeId);
    }

    // Test 8
    // UpdateCardAsync ska tilldela användare via användarnamn när AssignedTo är satt
    [Fact]
    public async Task UpdateCardAsync_WhenAssignedToIsUsername_SetsAssigneeId()
    {
        // Arrange
        var card = new Card { Id = 1, Title = "Kort", ListId = 2 };
        var user = new User { Id = 7, Username = "testuser" };

        _cardRepoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(card);

        _userRepoMock
            .Setup(r => r.GetByUsernameAsync("testuser"))
            .ReturnsAsync(user);

        _cardRepoMock
            .Setup(r => r.UpdateAsync(It.IsAny<Card>()))
            .ReturnsAsync((Card c) => c);

        var sut = CreateSut();
        var request = new UpdateCardRequest { Title = "Kort", AssignedTo = "testuser" };

        // Act
        var result = await sut.UpdateCardAsync(id: 1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(7, result.AssigneeId);
    }

    // Test 9
    // MoveCardAsync ska returnera null om kortet inte finns
    [Fact]
    public async Task MoveCardAsync_WhenCardDoesNotExist_ReturnsNull()
    {
        // Arrange
        _cardRepoMock
            .Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((Card?)null);

        var sut = CreateSut();
        var request = new MoveCardRequest { TargetListId = 3, Position = 0 };

        // Act
        var result = await sut.MoveCardAsync(id: 99, request);

        // Assert
        Assert.Null(result);
    }

    // Test 10
    // MoveCardAsync ska returnera null om mållistan inte finns
    [Fact]
    public async Task MoveCardAsync_WhenTargetListDoesNotExist_ReturnsNull()
    {
        // Arrange
        var card = new Card { Id = 1, Title = "Kort", ListId = 2 };

        _cardRepoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(card);

        _cardRepoMock
            .Setup(r => r.ListExistsAsync(99))
            .ReturnsAsync(false);

        var sut = CreateSut();
        var request = new MoveCardRequest { TargetListId = 99, Position = 0 };

        // Act
        var result = await sut.MoveCardAsync(id: 1, request);

        // Assert
        Assert.Null(result);
    }

    // Test 11
    // MoveCardAsync ska uppdatera ListId och Position vid lyckad flytt
    [Fact]
    public async Task MoveCardAsync_WhenValid_UpdatesListIdAndPosition()
    {
        // Arrange
        var card = new Card { Id = 1, Title = "Kort", ListId = 2, Position = 0 };

        _cardRepoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(card);

        _cardRepoMock
            .Setup(r => r.ListExistsAsync(5))
            .ReturnsAsync(true);

        _cardRepoMock
            .Setup(r => r.UpdateAsync(It.IsAny<Card>()))
            .ReturnsAsync((Card c) => c);

        var sut = CreateSut();
        var request = new MoveCardRequest { TargetListId = 5, Position = 2 };

        // Act
        var result = await sut.MoveCardAsync(id: 1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.ListId);
        Assert.Equal(2, result.Position);
    }
}
