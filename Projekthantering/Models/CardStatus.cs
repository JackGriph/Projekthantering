namespace Projekthantering.Models;

public static class CardStatus
{
    public const string Todo = "todo";
    public const string InProgress = "in_progress";
    public const string Done = "done";

    public static readonly IReadOnlyList<string> All = [Todo, InProgress, Done];

    public static bool IsValid(string status) => All.Contains(status);
}
