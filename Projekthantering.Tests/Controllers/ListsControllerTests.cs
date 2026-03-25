using Microsoft.AspNetCore.Mvc;
using Moq;
using Projekthantering.Controllers;
using Projekthantering.Services;
using Projekthantering.Shared.DTOs;

namespace Projekthantering.Tests.Controllers;

public class ListsControllerTests
{
    private readonly Mock<IListService> _listServiceMock = new();

    private ListsController CreateSut() =>
        new(_listServiceMock.Object);

    // Test 1
    // GetListsByBoard ska returnera NotFound när tavlan inte finns
    [Fact]
    public async Task GetListsByBoard_WhenBoardNotFound_ReturnsNotFound()
    {
        // Arrange
        _listServiceMock
            .Setup(s => s.BoardExistsAsync(99))
            .ReturnsAsync(false);

        var sut = CreateSut();

        // Act
        var result = await sut.GetListsByBoard(99);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    // Test 2
    // GetListsByBoard ska returnera OkObjectResult med listor när tavlan finns
    [Fact]
    public async Task GetListsByBoard_WhenBoardExists_ReturnsOkWithLists()
    {
        // Arrange
        _listServiceMock
            .Setup(s => s.BoardExistsAsync(1))
            .ReturnsAsync(true);

        _listServiceMock
            .Setup(s => s.GetListsByBoardAsync(1))
            .ReturnsAsync(new List<BoardListDto>
            {
                new() { Id = 1, Title = "Att göra" },
                new() { Id = 2, Title = "Klart" }
            });

        var sut = CreateSut();

        // Act
        var result = await sut.GetListsByBoard(1);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<List<BoardListDto>>(ok.Value);
        Assert.Equal(2, body.Count);
    }

    // Test 3
    // CreateList ska returnera NotFound när tavlan inte finns
    [Fact]
    public async Task CreateList_WhenBoardNotFound_ReturnsNotFound()
    {
        // Arrange
        _listServiceMock
            .Setup(s => s.BoardExistsAsync(99))
            .ReturnsAsync(false);

        var sut = CreateSut();

        // Act
        var result = await sut.CreateList(99, new CreateListRequest { Title = "Lista" });

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    // Test 4
    // CreateList ska returnera CreatedAtActionResult med den skapade listan
    [Fact]
    public async Task CreateList_WhenBoardExists_ReturnsCreated()
    {
        // Arrange
        _listServiceMock
            .Setup(s => s.BoardExistsAsync(1))
            .ReturnsAsync(true);

        _listServiceMock
            .Setup(s => s.CreateListAsync(1, It.IsAny<CreateListRequest>()))
            .ReturnsAsync(new BoardListDto { Id = 3, Title = "Ny lista", Position = 0 });

        var sut = CreateSut();

        // Act
        var result = await sut.CreateList(1, new CreateListRequest { Title = "Ny lista" });

        // Assert
        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var body = Assert.IsType<BoardListDto>(created.Value);
        Assert.Equal("Ny lista", body.Title);
    }

    // Test 5
    // UpdateList ska returnera NotFound när listan inte finns
    [Fact]
    public async Task UpdateList_WhenListNotFound_ReturnsNotFound()
    {
        // Arrange
        _listServiceMock
            .Setup(s => s.UpdateListAsync(99, It.IsAny<UpdateListRequest>()))
            .ReturnsAsync((BoardListDto?)null);

        var sut = CreateSut();

        // Act
        var result = await sut.UpdateList(99, new UpdateListRequest { Title = "Ny titel", Position = 0 });

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    // Test 6
    // DeleteList ska returnera NoContent när listan raderas
    [Fact]
    public async Task DeleteList_WhenDeleted_ReturnsNoContent()
    {
        // Arrange
        _listServiceMock
            .Setup(s => s.DeleteListAsync(1))
            .ReturnsAsync(true);

        var sut = CreateSut();

        // Act
        var result = await sut.DeleteList(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    // Test 7
    // DeleteList ska returnera NotFound när listan inte finns
    [Fact]
    public async Task DeleteList_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _listServiceMock
            .Setup(s => s.DeleteListAsync(99))
            .ReturnsAsync(false);

        var sut = CreateSut();

        // Act
        var result = await sut.DeleteList(99);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}
