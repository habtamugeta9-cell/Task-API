namespace TaskAPi.Domain;

public sealed class TaskItem
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public bool IsCompleted { get;private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public TaskItem(
        string title,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(
                "Task title is required",
                nameof(title));
        }

        title = title.Trim();

        if (title.Length > 200)
        {
            throw new ArgumentException(
                "Task title is too long; it cannot be longer than 200 characters",
                nameof(title));
        }

        if (description is not null)
        {
            description = description.Trim();

            if (description.Length > 2000)
            {
                throw new ArgumentException(
                    "Task description is too long; it cannot be longer than 2000 characters",
                    nameof(description));
            }
        }

        Title = title;
        Description = description;
        CreatedAt = DateTimeOffset.Now;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Complete()
    {
        if (IsCompleted)
        {
            return;
        }

        IsCompleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Reopen()
    {
        if (!IsCompleted)
        {
            return;
        }

        IsCompleted = false;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    
    
    
    
}

