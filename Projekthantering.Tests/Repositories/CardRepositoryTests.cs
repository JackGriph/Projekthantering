using Microsoft.EntityFrameworkCore;
using Projekthantering.Data;
using Projekthantering.Models;
using Projekthantering.Repositories;

namespace Projekthantering.Tests.Repositories;

public class CardRepositoryTests
{
    private static AppDbContext CreateContext(string dbName) =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options);

    private static async Task<(AppDbContext context, BoardList list)> SetupListAsync(string dbName)
    {
        var context = CreateContext(dbName);
        var user = new User { Username = "ägare", Email = "agare@example.com", PasswordHash = "hash" };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var board = new Board { Title = "Tavla", OwnerId = user.Id };
        context.Boards.Add(board);
        await context.SaveChangesAsync();
        var list = new BoardList { Title = "Lista", BoardId = board.Id, Position = 0 };
        context.BoardLists.Add(list);
        await context.SaveChangesAsync();
        return (context, list);
    }

    // Test 1
    // CreateAsync ska spara kortet och returnera det med Id
    [Fact]
    public async Task CreateAsync_WhenCalled_SavesAndReturnsCard()
    {
        var (context, list) = await SetupListAsync(nameof(CreateAsync_WhenCalled_SavesAndReturnsCard));
        await using (context)
        {
            var repo = new CardRepository(context);
            var card = new Card { Title = "Nytt kort", ListId = list.Id, Position = 0 };

            var result = await repo.CreateAsync(card);

            Assert.True(result.Id > 0);
            Assert.Equal("Nytt kort", result.Title);
        }
    }

    // Test 2
    // GetByIdAsync ska returnera null om kortet inte finns
    [Fact]
    public async Task GetByIdAsync_WhenCardDoesNotExist_ReturnsNull()
    {
        await using var context = CreateContext(nameof(GetByIdAsync_WhenCardDoesNotExist_ReturnsNull));
        var repo = new CardRepository(context);

        var result = await repo.GetByIdAsync(99);

        Assert.Null(result);
    }

    // Test 3
    // GetByListIdAsync ska returnera kort sorterade efter position
    [Fact]
    public async Task GetByListIdAsync_WhenListHasCards_ReturnsCardsOrderedByPosition()
    {
        var (context, list) = await SetupListAsync(nameof(GetByListIdAsync_WhenListHasCards_ReturnsCardsOrderedByPosition));
        await using (context)
        {
            context.Cards.AddRange(
                new Card { Title = "Tredje", ListId = list.Id, Position = 2 },
                new Card { Title = "Första", ListId = list.Id, Position = 0 },
                new Card { Title = "Andra", ListId = list.Id, Position = 1 }
            );
            await context.SaveChangesAsync();

            var repo = new CardRepository(context);
            var result = await repo.GetByListIdAsync(list.Id);

            Assert.Equal(3, result.Count);
            Assert.Equal("Första", result[0].Title);
            Assert.Equal("Andra", result[1].Title);
            Assert.Equal("Tredje", result[2].Title);
        }
    }

    // Test 4
    // ListExistsAsync ska returnera true om listan finns
    [Fact]
    public async Task ListExistsAsync_WhenListExists_ReturnsTrue()
    {
        var (context, list) = await SetupListAsync(nameof(ListExistsAsync_WhenListExists_ReturnsTrue));
        await using (context)
        {
            var repo = new CardRepository(context);

            var result = await repo.ListExistsAsync(list.Id);

            Assert.True(result);
        }
    }

    // Test 5
    // ListExistsAsync ska returnera false om listan inte finns
    [Fact]
    public async Task ListExistsAsync_WhenListDoesNotExist_ReturnsFalse()
    {
        await using var context = CreateContext(nameof(ListExistsAsync_WhenListDoesNotExist_ReturnsFalse));
        var repo = new CardRepository(context);

        var result = await repo.ListExistsAsync(99);

        Assert.False(result);
    }

    // Test 6
    // DeleteAsync ska ta bort kortet och returnera true
    [Fact]
    public async Task DeleteAsync_WhenCardExists_DeletesAndReturnsTrue()
    {
        var (context, list) = await SetupListAsync(nameof(DeleteAsync_WhenCardExists_DeletesAndReturnsTrue));
        await using (context)
        {
            var card = new Card { Title = "Kort att ta bort", ListId = list.Id, Position = 0 };
            context.Cards.Add(card);
            await context.SaveChangesAsync();

            var repo = new CardRepository(context);
            var result = await repo.DeleteAsync(card.Id);

            Assert.True(result);
            Assert.Null(await repo.GetByIdAsync(card.Id));
        }
    }

    // Test 7
    // DeleteAsync ska returnera false om kortet inte finns
    [Fact]
    public async Task DeleteAsync_WhenCardDoesNotExist_ReturnsFalse()
    {
        await using var context = CreateContext(nameof(DeleteAsync_WhenCardDoesNotExist_ReturnsFalse));
        var repo = new CardRepository(context);

        var result = await repo.DeleteAsync(99);

        Assert.False(result);
    }

    // Test 8
    // UpdateAsync ska spara ändringarna i databasen
    [Fact]
    public async Task UpdateAsync_WhenCalled_PersistsChanges()
    {
        var (context, list) = await SetupListAsync(nameof(UpdateAsync_WhenCalled_PersistsChanges));
        await using (context)
        {
            var card = new Card { Title = "Ursprunglig titel", ListId = list.Id, Position = 0 };
            context.Cards.Add(card);
            await context.SaveChangesAsync();

            var repo = new CardRepository(context);
            card.Title = "Uppdaterad titel";
            card.Description = "Ny beskrivning";
            await repo.UpdateAsync(card);

            var updated = await repo.GetByIdAsync(card.Id);
            Assert.NotNull(updated);
            Assert.Equal("Uppdaterad titel", updated.Title);
            Assert.Equal("Ny beskrivning", updated.Description);
        }
    }
}
