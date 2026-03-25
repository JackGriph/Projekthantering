using Microsoft.EntityFrameworkCore;
using Projekthantering.Data;
using Projekthantering.Models;
using Projekthantering.Repositories;

namespace Projekthantering.Tests.Repositories;

public class ListRepositoryTests
{
    private static AppDbContext CreateContext(string dbName) =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options);

    private static async Task<(AppDbContext context, Board board)> SetupBoardAsync(string dbName)
    {
        var context = CreateContext(dbName);
        var user = new User { Username = "ägare", Email = "agare@example.com", PasswordHash = "hash" };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var board = new Board { Title = "Tavla", OwnerId = user.Id };
        context.Boards.Add(board);
        await context.SaveChangesAsync();
        return (context, board);
    }

    // Test 1
    // CreateAsync ska spara listan och returnera den med Id
    [Fact]
    public async Task CreateAsync_WhenCalled_SavesAndReturnsList()
    {
        var (context, board) = await SetupBoardAsync(nameof(CreateAsync_WhenCalled_SavesAndReturnsList));
        await using (context)
        {
            var repo = new ListRepository(context);
            var list = new BoardList { Title = "Att göra", BoardId = board.Id, Position = 0 };

            var result = await repo.CreateAsync(list);

            Assert.True(result.Id > 0);
            Assert.Equal("Att göra", result.Title);
        }
    }

    // Test 2
    // GetByIdAsync ska returnera null om listan inte finns
    [Fact]
    public async Task GetByIdAsync_WhenListDoesNotExist_ReturnsNull()
    {
        await using var context = CreateContext(nameof(GetByIdAsync_WhenListDoesNotExist_ReturnsNull));
        var repo = new ListRepository(context);

        var result = await repo.GetByIdAsync(99);

        Assert.Null(result);
    }

    // Test 3
    // GetByBoardIdAsync ska returnera listor sorterade efter position
    [Fact]
    public async Task GetByBoardIdAsync_WhenBoardHasLists_ReturnsListsOrderedByPosition()
    {
        var (context, board) = await SetupBoardAsync(nameof(GetByBoardIdAsync_WhenBoardHasLists_ReturnsListsOrderedByPosition));
        await using (context)
        {
            context.BoardLists.AddRange(
                new BoardList { Title = "Tredje", BoardId = board.Id, Position = 2 },
                new BoardList { Title = "Första", BoardId = board.Id, Position = 0 },
                new BoardList { Title = "Andra", BoardId = board.Id, Position = 1 }
            );
            await context.SaveChangesAsync();

            var repo = new ListRepository(context);
            var result = await repo.GetByBoardIdAsync(board.Id);

            Assert.Equal(3, result.Count);
            Assert.Equal("Första", result[0].Title);
            Assert.Equal("Andra", result[1].Title);
            Assert.Equal("Tredje", result[2].Title);
        }
    }

    // Test 4
    // BoardExistsAsync ska returnera true om tavlan finns
    [Fact]
    public async Task BoardExistsAsync_WhenBoardExists_ReturnsTrue()
    {
        var (context, board) = await SetupBoardAsync(nameof(BoardExistsAsync_WhenBoardExists_ReturnsTrue));
        await using (context)
        {
            var repo = new ListRepository(context);

            var result = await repo.BoardExistsAsync(board.Id);

            Assert.True(result);
        }
    }

    // Test 5
    // BoardExistsAsync ska returnera false om tavlan inte finns
    [Fact]
    public async Task BoardExistsAsync_WhenBoardDoesNotExist_ReturnsFalse()
    {
        await using var context = CreateContext(nameof(BoardExistsAsync_WhenBoardDoesNotExist_ReturnsFalse));
        var repo = new ListRepository(context);

        var result = await repo.BoardExistsAsync(99);

        Assert.False(result);
    }

    // Test 6
    // DeleteAsync ska ta bort listan och returnera true
    [Fact]
    public async Task DeleteAsync_WhenListExists_DeletesAndReturnsTrue()
    {
        var (context, board) = await SetupBoardAsync(nameof(DeleteAsync_WhenListExists_DeletesAndReturnsTrue));
        await using (context)
        {
            var list = new BoardList { Title = "Lista", BoardId = board.Id, Position = 0 };
            context.BoardLists.Add(list);
            await context.SaveChangesAsync();

            var repo = new ListRepository(context);
            var result = await repo.DeleteAsync(list.Id);

            Assert.True(result);
            Assert.Null(await repo.GetByIdAsync(list.Id));
        }
    }
}
