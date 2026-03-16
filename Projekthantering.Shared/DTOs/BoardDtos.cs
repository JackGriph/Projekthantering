using System.ComponentModel.DataAnnotations;

namespace Projekthantering.Shared.DTOs;

public class BoardDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateBoardRequest
{
    [Required(ErrorMessage = "Titel krävs.")]
    [MinLength(1, ErrorMessage = "Titel får inte vara tom.")]
    [MaxLength(100, ErrorMessage = "Titel får vara max 100 tecken.")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Beskrivning får vara max 500 tecken.")]
    public string? Description { get; set; }
}

public class UpdateBoardRequest
{
    [Required(ErrorMessage = "Titel krävs.")]
    [MinLength(1, ErrorMessage = "Titel får inte vara tom.")]
    [MaxLength(100, ErrorMessage = "Titel får vara max 100 tecken.")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Beskrivning får vara max 500 tecken.")]
    public string? Description { get; set; }
}
