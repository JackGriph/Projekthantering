namespace Projekthantering.Models;

public class BoardMember
{
    public int Id { get; set; }
    public int BoardId { get; set; }
    public int UserId { get; set; }

    public Board Board { get; set; } = null!;
    public User User { get; set; } = null!;
}
