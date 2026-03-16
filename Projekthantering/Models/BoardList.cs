namespace Projekthantering.Models;

public class BoardList
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int BoardId { get; set; }
    public int Position { get; set; }

    public Board Board { get; set; } = null!;
    public ICollection<Card> Cards { get; set; } = new List<Card>();
}
