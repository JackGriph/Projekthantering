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
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateBoardRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}
