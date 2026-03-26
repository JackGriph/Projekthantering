using Moq;
using Projekthantering.Models;
using Projekthantering.Repositories;
using Projekthantering.Services;
using Projekthantering.Shared.DTOs;

namespace Projekthantering.Tests;

public class ListServiceTests
{
    private readonly Mock<IListRepository> _listRepoMock = new();

    private ListService CreateSut() =>
        new(_listRepoMock.Object);

    private static BoardList MakeList(int id, int boardId, int position, string title = "Lista") => new()
    {
        Id = id,
        Title = title,
        BoardId = boardId,
        Position = position,
        Cards = new List<Card>()
    };

    // Test 1
    // CreateListAsync ska tilldela position max+1 när tavlan redan har listor
    [Fact]
    public async Task CreateListAsync_WhenBoardHasExistingLists_AssignsNextPosition()
    {
        // Arrange
        var existingLists = new List<BoardList>
        {
            MakeList(id: 1, boardId: 10, position: 0),
            MakeList(id: 2, boardId: 10, position: 1)
        };

        _listRepoMock
            .Setup(r => r.GetByBoardIdAsync(10))
            .ReturnsAsync(existingLists);

        _listRepoMock
            .Setup(r => r.CreateAsync(It.IsAny<BoardList>()))
            .ReturnsAsync((BoardList l) => l);

        var sut = CreateSut();
        var request = new CreateListRequest { Title = "Ny lista" };

        // Act
        var result = await sut.CreateListAsync(boardId: 10, request);

        // Assert
        Assert.Equal(2, result.Position); // max(0,1) + 1 = 2
    }

    // Test 2
    // GetListByIdAsync ska returnera null om listan inte finns
    [Fact]
    public async Task GetListByIdAsync_WhenListDoesNotExist_ReturnsNull()
    {
        // Arrange
        _listRepoMock
            .Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((BoardList?)null);

        var sut = CreateSut();

        // Act
        var result = await sut.GetListByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    // Test 3
    // GetListByIdAsync ska returnera BoardListDto om listan finns
    [Fact]
    public async Task GetListByIdAsync_WhenListExists_ReturnsBoardListDto()
    {
        // Arrange
        var list = MakeList(id: 1, boardId: 5, position: 0, title: "Att göra");

        _listRepoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(list);

        var sut = CreateSut();

        // Act
        var result = await sut.GetListByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Att göra", result.Title);
        Assert.Equal(0, result.Position);
    }

    // Test 4
    // GetListsByBoardAsync ska returnera alla listor för en tavla
    [Fact]
    public async Task GetListsByBoardAsync_WhenBoardHasLists_ReturnsAllLists()
    {
        // Arrange
        var lists = new List<BoardList>
        {
            MakeList(id: 1, boardId: 3, position: 0, title: "Att göra"),
            MakeList(id: 2, boardId: 3, position: 1, title: "Pågående"),
            MakeList(id: 3, boardId: 3, position: 2, title: "Klart")
        };

        _listRepoMock
            .Setup(r => r.GetByBoardIdAsync(3))
            .ReturnsAsync(lists);

        var sut = CreateSut();

        // Act
        var result = await sut.GetListsByBoardAsync(boardId: 3);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("Att göra", result[0].Title);
        Assert.Equal("Klart", result[2].Title);
    }

    // Test 5
    // UpdateListAsync ska returnera null om listan inte finns
    [Fact]
    public async Task UpdateListAsync_WhenListDoesNotExist_ReturnsNull()
    {
        // Arrange
        _listRepoMock
            .Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((BoardList?)null);

        var sut = CreateSut();
        var request = new UpdateListRequest { Title = "Ny titel", Position = 0 };

        // Act
        var result = await sut.UpdateListAsync(id: 99, request);

        // Assert
        Assert.Null(result);
    }

    // Test 6
    // UpdateListAsync ska uppdatera titel och position
    [Fact]
    public async Task UpdateListAsync_WhenListExists_UpdatesTitleAndPosition()
    {
        // Arrange
        var list = MakeList(id: 1, boardId: 5, position: 0, title: "Gammal titel");

        _listRepoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(list);

        _listRepoMock
            .Setup(r => r.UpdateAsync(It.IsAny<BoardList>()))
            .ReturnsAsync((BoardList l) => l);

        var sut = CreateSut();
        var request = new UpdateListRequest { Title = "Ny titel", Position = 3 };

        // Act
        var result = await sut.UpdateListAsync(id: 1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Ny titel", result.Title);
        Assert.Equal(3, result.Position);
    }
}
