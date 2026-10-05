using TaskApi.Domain;

namespace TaskApi.DTOs;

public sealed class TaskResponse
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public bool IsCompleted { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? UpdatedAt { get; init; }

    public Guid Version { get; init; }

    public static TaskResponse FromDomain(
        TaskItem task)
    {
        return new TaskResponse
        {
            Id = task.Id,
            UserId = task.UserId,
            Title = task.Title,
            Description = task.Description,
            IsCompleted = task.IsCompleted,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            Version = task.Version
        };
    }
}