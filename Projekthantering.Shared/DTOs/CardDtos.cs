using System.ComponentModel.DataAnnotations;

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
    [Required(ErrorMessage = "Titel krävs.")]
    [MinLength(1, ErrorMessage = "Titel får inte vara tom.")]
    [MaxLength(200, ErrorMessage = "Titel får vara max 200 tecken.")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000, ErrorMessage = "Beskrivning får vara max 2000 tecken.")]
    public string? Description { get; set; }
}

public class UpdateCardRequest
{
    [Required(ErrorMessage = "Titel krävs.")]
    [MinLength(1, ErrorMessage = "Titel får inte vara tom.")]
    [MaxLength(200, ErrorMessage = "Titel får vara max 200 tecken.")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000, ErrorMessage = "Beskrivning får vara max 2000 tecken.")]
    public string? Description { get; set; }

    public int? AssigneeId { get; set; }
    public DateTime? DueDate { get; set; }
}

public class MoveCardRequest
{
    [Required]
    public int TargetListId { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Position måste vara ett positivt tal.")]
    public int NewPosition { get; set; }
}
