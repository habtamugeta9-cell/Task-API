namespace TaskApi.Requests;

/// <summary>
/// Represents the data required to create a task.
/// </summary>
public sealed class CreateTaskRequest
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; set; }
}