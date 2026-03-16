namespace Projekthantering.Models;

public class Board
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OwnerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User Owner { get; set; } = null!;
    public ICollection<BoardMember> Members { get; set; } = new List<BoardMember>();
    public ICollection<BoardList> Lists { get; set; } = new List<BoardList>();
}
