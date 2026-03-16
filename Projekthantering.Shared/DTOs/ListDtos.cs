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
    public string Title { get; set; } = string.Empty;
}

public class UpdateListRequest
{
    public string Title { get; set; } = string.Empty;
    public int Position { get; set; }
}
