using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Projekthantering.Controllers;
using Projekthantering.Services;
using Projekthantering.Shared.DTOs;

namespace Projekthantering.Tests.Controllers;

public class BoardsControllerTests
{
    private readonly Mock<IBoardService> _boardServiceMock = new();

    private BoardsController CreateSut(int userId = 1)
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var controller = new BoardsController(_boardServiceMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
                }
            }
        };
        return controller;
    }

    // Test 1
    // GetBoards ska returnera OkObjectResult med listan av tavlor
    [Fact]
    public async Task GetBoards_WhenCalled_ReturnsOkWithBoards()
    {
        // Arrange
        var boards = new List<BoardDto>
        {
            new() { Id = 1, Title = "Tavla A" },
            new() { Id = 2, Title = "Tavla B" }
        };
        _boardServiceMock
            .Setup(s => s.GetBoardsByUserAsync(1))
            .ReturnsAsync(boards);

        var sut = CreateSut(userId: 1);

        // Act
        var result = await sut.GetBoards();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<List<BoardDto>>(ok.Value);
        Assert.Equal(2, body.Count);
    }

    // Test 2
    // GetBoard ska returnera OkObjectResult när tavlan finns och användaren har åtkomst
    [Fact]
    public async Task GetBoard_WhenBoardExists_ReturnsOk()
    {
        // Arrange
        var board = new BoardDto { Id = 1, Title = "Min tavla" };
        _boardServiceMock
            .Setup(s => s.GetBoardByIdAsync(1, 1))
            .ReturnsAsync(board);

        var sut = CreateSut(userId: 1);

        // Act
        var result = await sut.GetBoard(1);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<BoardDto>(ok.Value);
        Assert.Equal("Min tavla", body.Title);
    }

    // Test 3
    // GetBoard ska returnera NotFound när tavlan inte finns eller saknar åtkomst
    [Fact]
    public async Task GetBoard_WhenBoardNotFound_ReturnsNotFound()
    {
        // Arrange
        _boardServiceMock
            .Setup(s => s.GetBoardByIdAsync(99, 1))
            .ReturnsAsync((BoardDto?)null);

        var sut = CreateSut(userId: 1);

        // Act
        var result = await sut.GetBoard(99);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    // Test 4
    // CreateBoard ska returnera CreatedAtActionResult med den skapade tavlan
    [Fact]
    public async Task CreateBoard_WhenValid_ReturnsCreated()
    {
        // Arrange
        var board = new BoardDto { Id = 5, Title = "Ny tavla" };
        _boardServiceMock
            .Setup(s => s.CreateBoardAsync(It.IsAny<CreateBoardRequest>(), 1))
            .ReturnsAsync(board);

        var sut = CreateSut(userId: 1);

        // Act
        var result = await sut.CreateBoard(new CreateBoardRequest { Title = "Ny tavla" });

        // Assert
        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var body = Assert.IsType<BoardDto>(created.Value);
        Assert.Equal("Ny tavla", body.Title);
    }

    // Test 5
    // UpdateBoard ska returnera NotFound när tavlan inte finns eller inte ägs av användaren
    [Fact]
    public async Task UpdateBoard_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _boardServiceMock
            .Setup(s => s.UpdateBoardAsync(99, It.IsAny<UpdateBoardRequest>(), 1))
            .ReturnsAsync((BoardDto?)null);

        var sut = CreateSut(userId: 1);

        // Act
        var result = await sut.UpdateBoard(99, new UpdateBoardRequest { Title = "Ny titel" });

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    // Test 6
    // DeleteBoard ska returnera NoContent när tavlan raderas
    [Fact]
    public async Task DeleteBoard_WhenDeleted_ReturnsNoContent()
    {
        // Arrange
        _boardServiceMock
            .Setup(s => s.DeleteBoardAsync(1, 1))
            .ReturnsAsync(true);

        var sut = CreateSut(userId: 1);

        // Act
        var result = await sut.DeleteBoard(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    // Test 7
    // DeleteBoard ska returnera NotFound när tavlan inte finns
    [Fact]
    public async Task DeleteBoard_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _boardServiceMock
            .Setup(s => s.DeleteBoardAsync(99, 1))
            .ReturnsAsync(false);

        var sut = CreateSut(userId: 1);

        // Act
        var result = await sut.DeleteBoard(99);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}
