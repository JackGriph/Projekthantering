using System.ComponentModel.DataAnnotations;

namespace Projekthantering.Shared.DTOs;

public class BoardListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Position { get; set; }
    public List<CardDto> Cards { get; set; } = new();
}

public class CreateListRequest
{
    [Required(ErrorMessage = "Titel krävs.")]
    [MinLength(1, ErrorMessage = "Titel får inte vara tom.")]
    [MaxLength(100, ErrorMessage = "Titel får vara max 100 tecken.")]
    public string Title { get; set; } = string.Empty;
}

public class UpdateListRequest
{
    [Required(ErrorMessage = "Titel krävs.")]
    [MinLength(1, ErrorMessage = "Titel får inte vara tom.")]
    [MaxLength(100, ErrorMessage = "Titel får vara max 100 tecken.")]
    public string Title { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "Position måste vara ett positivt tal.")]
    public int Position { get; set; }
}
