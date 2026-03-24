namespace Projekthantering.Models;

public class Card
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ListId { get; set; }
    public int Position { get; set; }
    public string Status { get; set; } = CardStatus.Todo;
    public int? AssigneeId { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public BoardList List { get; set; } = null!;
    public User? Assignee { get; set; }
}
