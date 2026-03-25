using Microsoft.AspNetCore.Mvc;
using Moq;
using Projekthantering.Controllers;
using Projekthantering.Services;
using Projekthantering.Shared.DTOs;

namespace Projekthantering.Tests.Controllers;

public class CardsControllerTests
{
    private readonly Mock<ICardService> _cardServiceMock = new();

    private CardsController CreateSut() =>
        new(_cardServiceMock.Object);

    // Test 1
    // GetCardsByList ska returnera NotFound när listan inte finns
    [Fact]
    public async Task GetCardsByList_WhenListNotFound_ReturnsNotFound()
    {
        // Arrange
        _cardServiceMock
            .Setup(s => s.ListExistsAsync(99))
            .ReturnsAsync(false);

        var sut = CreateSut();

        // Act
        var result = await sut.GetCardsByList(99);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    // Test 2
    // GetCardsByList ska returnera OkObjectResult med kort när listan finns
    [Fact]
    public async Task GetCardsByList_WhenListExists_ReturnsOkWithCards()
    {
        // Arrange
        _cardServiceMock
            .Setup(s => s.ListExistsAsync(1))
            .ReturnsAsync(true);

        _cardServiceMock
            .Setup(s => s.GetCardsByListAsync(1))
            .ReturnsAsync(new List<CardDto>
            {
                new() { Id = 1, Title = "Kort A" },
                new() { Id = 2, Title = "Kort B" }
            });

        var sut = CreateSut();

        // Act
        var result = await sut.GetCardsByList(1);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<List<CardDto>>(ok.Value);
        Assert.Equal(2, body.Count);
    }

    // Test 3
    // CreateCard ska returnera NotFound när listan inte finns
    [Fact]
    public async Task CreateCard_WhenListNotFound_ReturnsNotFound()
    {
        // Arrange
        _cardServiceMock
            .Setup(s => s.ListExistsAsync(99))
            .ReturnsAsync(false);

        var sut = CreateSut();

        // Act
        var result = await sut.CreateCard(99, new CreateCardRequest { Title = "Kort" });

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    // Test 4
    // CreateCard ska returnera CreatedAtActionResult med det skapade kortet
    [Fact]
    public async Task CreateCard_WhenListExists_ReturnsCreated()
    {
        // Arrange
        _cardServiceMock
            .Setup(s => s.ListExistsAsync(1))
            .ReturnsAsync(true);

        _cardServiceMock
            .Setup(s => s.CreateCardAsync(1, It.IsAny<CreateCardRequest>()))
            .ReturnsAsync(new CardDto { Id = 5, Title = "Nytt kort", ListId = 1 });

        var sut = CreateSut();

        // Act
        var result = await sut.CreateCard(1, new CreateCardRequest { Title = "Nytt kort" });

        // Assert
        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var body = Assert.IsType<CardDto>(created.Value);
        Assert.Equal("Nytt kort", body.Title);
    }

    // Test 5
    // UpdateCard ska returnera NotFound när kortet inte finns
    [Fact]
    public async Task UpdateCard_WhenCardNotFound_ReturnsNotFound()
    {
        // Arrange
        _cardServiceMock
            .Setup(s => s.UpdateCardAsync(99, It.IsAny<UpdateCardRequest>()))
            .ReturnsAsync((CardDto?)null);

        var sut = CreateSut();

        // Act
        var result = await sut.UpdateCard(99, new UpdateCardRequest { Title = "Titel" });

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    // Test 6
    // DeleteCard ska returnera NoContent när kortet raderas
    [Fact]
    public async Task DeleteCard_WhenDeleted_ReturnsNoContent()
    {
        // Arrange
        _cardServiceMock
            .Setup(s => s.DeleteCardAsync(1))
            .ReturnsAsync(true);

        var sut = CreateSut();

        // Act
        var result = await sut.DeleteCard(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    // Test 7
    // DeleteCard ska returnera NotFound när kortet inte finns
    [Fact]
    public async Task DeleteCard_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _cardServiceMock
            .Setup(s => s.DeleteCardAsync(99))
            .ReturnsAsync(false);

        var sut = CreateSut();

        // Act
        var result = await sut.DeleteCard(99);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    // Test 8
    // MoveCard ska returnera NotFound när kortet eller mållistan inte finns
    [Fact]
    public async Task MoveCard_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _cardServiceMock
            .Setup(s => s.MoveCardAsync(99, It.IsAny<MoveCardRequest>()))
            .ReturnsAsync((CardDto?)null);

        var sut = CreateSut();

        // Act
        var result = await sut.MoveCard(99, new MoveCardRequest { TargetListId = 1, Position = 0 });

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    // Test 9
    // MoveCard ska returnera OkObjectResult med det flyttade kortet
    [Fact]
    public async Task MoveCard_WhenValid_ReturnsOkWithCard()
    {
        // Arrange
        var movedCard = new CardDto { Id = 1, Title = "Kort", ListId = 5, Position = 2 };
        _cardServiceMock
            .Setup(s => s.MoveCardAsync(1, It.IsAny<MoveCardRequest>()))
            .ReturnsAsync(movedCard);

        var sut = CreateSut();

        // Act
        var result = await sut.MoveCard(1, new MoveCardRequest { TargetListId = 5, Position = 2 });

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<CardDto>(ok.Value);
        Assert.Equal(5, body.ListId);
        Assert.Equal(2, body.Position);
    }
}
