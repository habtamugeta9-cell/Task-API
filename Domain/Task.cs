namespace TaskApi.Domain;

public sealed class Task
{
    public Guid Id { get; init; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; set; }
}
