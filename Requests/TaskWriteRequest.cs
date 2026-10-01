namespace TaskApi.Requests;

/// <summary>
/// Represents the data required to create or update a task.
/// </summary>
public sealed class TaskWriteRequest
{
    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }
}   