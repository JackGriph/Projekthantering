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
    // CreateCardAsync ska tilldela position max+1 när listan redan har kort
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
    // CreateListAsync ska sätta position 0 när tavlan inte har några listor
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
}
