using Microsoft.EntityFrameworkCore;
using Projekthantering.Data;
using Projekthantering.Models;

namespace Projekthantering.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
        => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetByIdAsync(int id)
        => await _context.Users.FindAsync(id);

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> EmailExistsAsync(string email)
        => await _context.Users.AnyAsync(u => u.Email == email);

    public async Task<bool> UsernameExistsAsync(string username)
        => await _context.Users.AnyAsync(u => u.Username == username);

    public async Task<List<User>> GetAllAsync()
        => await _context.Users.ToListAsync();

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        => await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}
