using TaskApi.Domain;

namespace TaskApi.Responses;

public sealed class TaskResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } =  string.Empty;
    public string? Description { get; init; } = null;
    public bool IsCompleted { get; init; } =  false;
    public DateTimeOffset CreatedAt { get; init; } 
    public DateTimeOffset? UpdatedAt { get; private set; }
    public static TaskResponse FromDomain(TaskItem task)
    {
        return new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IsCompleted = task.IsCompleted,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
    
     
}

