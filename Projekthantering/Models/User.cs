namespace Projekthantering.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User"; // "User" eller "Admin"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Board> OwnedBoards { get; set; } = new List<Board>();
    public ICollection<BoardMember> BoardMemberships { get; set; } = new List<BoardMember>();
    public ICollection<Card> AssignedCards { get; set; } = new List<Card>();
}
