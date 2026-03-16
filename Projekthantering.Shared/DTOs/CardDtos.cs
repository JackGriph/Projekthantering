namespace Projekthantering.Shared.DTOs;

public class CardDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ListId { get; set; }
    public int Position { get; set; }
    public int? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCardRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateCardRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? AssigneeId { get; set; }
    public DateTime? DueDate { get; set; }
}

public class MoveCardRequest
{
    public int TargetListId { get; set; }
    public int NewPosition { get; set; }
}
