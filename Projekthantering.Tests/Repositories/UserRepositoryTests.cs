using Microsoft.EntityFrameworkCore;
using Projekthantering.Data;
using Projekthantering.Models;
using Projekthantering.Repositories;

namespace Projekthantering.Tests.Repositories;

public class UserRepositoryTests
{
    private static AppDbContext CreateContext(string dbName) =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options);

    // Test 1
    // CreateAsync ska spara användaren och returnera den med tilldelat Id
    [Fact]
    public async Task CreateAsync_WhenCalled_SavesAndReturnsUser()
    {
        await using var context = CreateContext(nameof(CreateAsync_WhenCalled_SavesAndReturnsUser));
        var repo = new UserRepository(context);
        var user = new User { Username = "testuser", Email = "test@example.com", PasswordHash = "hash" };

        var result = await repo.CreateAsync(user);

        Assert.True(result.Id > 0);
        Assert.Equal("testuser", result.Username);
    }

    // Test 2
    // GetByEmailAsync ska returnera rätt användare
    [Fact]
    public async Task GetByEmailAsync_WhenUserExists_ReturnsUser()
    {
        await using var context = CreateContext(nameof(GetByEmailAsync_WhenUserExists_ReturnsUser));
        var repo = new UserRepository(context);
        await repo.CreateAsync(new User { Username = "anna", Email = "anna@example.com", PasswordHash = "hash" });

        var result = await repo.GetByEmailAsync("anna@example.com");

        Assert.NotNull(result);
        Assert.Equal("anna", result.Username);
    }

    // Test 3
    // GetByEmailAsync ska returnera null om emailen inte finns
    [Fact]
    public async Task GetByEmailAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        await using var context = CreateContext(nameof(GetByEmailAsync_WhenUserDoesNotExist_ReturnsNull));
        var repo = new UserRepository(context);

        var result = await repo.GetByEmailAsync("ingen@example.com");

        Assert.Null(result);
    }

    // Test 4
    // EmailExistsAsync ska returnera true om emailen finns
    [Fact]
    public async Task EmailExistsAsync_WhenEmailExists_ReturnsTrue()
    {
        await using var context = CreateContext(nameof(EmailExistsAsync_WhenEmailExists_ReturnsTrue));
        var repo = new UserRepository(context);
        await repo.CreateAsync(new User { Username = "bob", Email = "bob@example.com", PasswordHash = "hash" });

        var result = await repo.EmailExistsAsync("bob@example.com");

        Assert.True(result);
    }

    // Test 5
    // EmailExistsAsync ska returnera false om emailen inte finns
    [Fact]
    public async Task EmailExistsAsync_WhenEmailDoesNotExist_ReturnsFalse()
    {
        await using var context = CreateContext(nameof(EmailExistsAsync_WhenEmailDoesNotExist_ReturnsFalse));
        var repo = new UserRepository(context);

        var result = await repo.EmailExistsAsync("ingen@example.com");

        Assert.False(result);
    }

    // Test 6
    // GetByUsernameAsync ska returnera rätt användare
    [Fact]
    public async Task GetByUsernameAsync_WhenUserExists_ReturnsUser()
    {
        await using var context = CreateContext(nameof(GetByUsernameAsync_WhenUserExists_ReturnsUser));
        var repo = new UserRepository(context);
        await repo.CreateAsync(new User { Username = "kalle", Email = "kalle@example.com", PasswordHash = "hash" });

        var result = await repo.GetByUsernameAsync("kalle");

        Assert.NotNull(result);
        Assert.Equal("kalle@example.com", result.Email);
    }

    // Test 7
    // UpdateAsync ska spara ändringar till databasen
    [Fact]
    public async Task UpdateAsync_WhenCalled_PersistsChanges()
    {
        await using var context = CreateContext(nameof(UpdateAsync_WhenCalled_PersistsChanges));
        var repo = new UserRepository(context);
        var user = await repo.CreateAsync(new User { Username = "original", Email = "u@example.com", PasswordHash = "hash" });

        user.Username = "uppdaterad";
        await repo.UpdateAsync(user);

        var updated = await repo.GetByIdAsync(user.Id);
        Assert.Equal("uppdaterad", updated!.Username);
    }
}
