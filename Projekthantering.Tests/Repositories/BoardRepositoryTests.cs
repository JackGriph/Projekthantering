using Microsoft.EntityFrameworkCore;
using Projekthantering.Data;
using Projekthantering.Models;
using Projekthantering.Repositories;

namespace Projekthantering.Tests.Repositories;

public class BoardRepositoryTests
{
    private static AppDbContext CreateContext(string dbName) =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options);

    private static User MakeUser(string username, string email) =>
        new() { Username = username, Email = email, PasswordHash = "hash" };

    // Test 1
    // CreateAsync ska spara tavlan och ladda Owner-navigationen
    [Fact]
    public async Task CreateAsync_WhenCalled_SavesBoardAndLoadsOwner()
    {
        await using var context = CreateContext(nameof(CreateAsync_WhenCalled_SavesBoardAndLoadsOwner));
        context.Users.Add(MakeUser("ägare", "agare@example.com"));
        await context.SaveChangesAsync();
        var owner = context.Users.First();

        var repo = new BoardRepository(context);
        var board = new Board { Title = "Min tavla", OwnerId = owner.Id };

        var result = await repo.CreateAsync(board);

        Assert.True(result.Id > 0);
        Assert.NotNull(result.Owner);
        Assert.Equal("ägare", result.Owner.Username);
    }

    // Test 2
    // GetByIdAsync ska returnera null om tavlan inte finns
    [Fact]
    public async Task GetByIdAsync_WhenBoardDoesNotExist_ReturnsNull()
    {
        await using var context = CreateContext(nameof(GetByIdAsync_WhenBoardDoesNotExist_ReturnsNull));
        var repo = new BoardRepository(context);

        var result = await repo.GetByIdAsync(99);

        Assert.Null(result);
    }

    // Test 3
    // GetByIdAsync ska returnera tavlan med listor och medlemmar inkluderade
    [Fact]
    public async Task GetByIdAsync_WhenBoardExists_ReturnsBoardWithIncludes()
    {
        await using var context = CreateContext(nameof(GetByIdAsync_WhenBoardExists_ReturnsBoardWithIncludes));
        var user = MakeUser("ägare", "agare@example.com");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var board = new Board { Title = "Tavla", OwnerId = user.Id };
        context.Boards.Add(board);
        await context.SaveChangesAsync();

        context.BoardLists.Add(new BoardList { Title = "Lista 1", BoardId = board.Id, Position = 0 });
        await context.SaveChangesAsync();

        var repo = new BoardRepository(context);
        var result = await repo.GetByIdAsync(board.Id);

        Assert.NotNull(result);
        Assert.Single(result.Lists);
        Assert.NotNull(result.Owner);
    }

    // Test 4
    // GetByUserIdAsync ska returnera tavlor där användaren är ägare
    [Fact]
    public async Task GetByUserIdAsync_WhenUserOwnsBoards_ReturnsThoseBoards()
    {
        await using var context = CreateContext(nameof(GetByUserIdAsync_WhenUserOwnsBoards_ReturnsThoseBoards));
        var user = MakeUser("ägare", "agare@example.com");
        var other = MakeUser("annan", "annan@example.com");
        context.Users.AddRange(user, other);
        await context.SaveChangesAsync();

        context.Boards.AddRange(
            new Board { Title = "Ägarens tavla", OwnerId = user.Id },
            new Board { Title = "Annans tavla", OwnerId = other.Id }
        );
        await context.SaveChangesAsync();

        var repo = new BoardRepository(context);
        var result = await repo.GetByUserIdAsync(user.Id);

        Assert.Single(result);
        Assert.Equal("Ägarens tavla", result[0].Title);
    }

    // Test 5
    // DeleteAsync ska ta bort tavlan och returnera true
    [Fact]
    public async Task DeleteAsync_WhenBoardExists_DeletesAndReturnsTrue()
    {
        await using var context = CreateContext(nameof(DeleteAsync_WhenBoardExists_DeletesAndReturnsTrue));
        var user = MakeUser("ägare", "agare@example.com");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var board = new Board { Title = "Tavla att ta bort", OwnerId = user.Id };
        context.Boards.Add(board);
        await context.SaveChangesAsync();

        var repo = new BoardRepository(context);
        var result = await repo.DeleteAsync(board.Id);

        Assert.True(result);
        Assert.Null(await repo.GetByIdAsync(board.Id));
    }

    // Test 6
    // DeleteAsync ska returnera false om tavlan inte finns
    [Fact]
    public async Task DeleteAsync_WhenBoardDoesNotExist_ReturnsFalse()
    {
        await using var context = CreateContext(nameof(DeleteAsync_WhenBoardDoesNotExist_ReturnsFalse));
        var repo = new BoardRepository(context);

        var result = await repo.DeleteAsync(99);

        Assert.False(result);
    }
}
