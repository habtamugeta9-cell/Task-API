namespace TaskApi.Requests;

/// <summary>
/// Represents the data required to update a task.
/// </summary>
public sealed class UpdateTaskRequest
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
}