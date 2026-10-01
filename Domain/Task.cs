namespace TaskApi.Domain;

public sealed class TaskItem
{
    public Guid Id { get; private set; }

    public string Title { get; private set; }

    public string? Description { get; private set; }

    public bool IsCompleted { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public TaskItem(
        string title,
        string? description = null)
    {
        ValidateTitle(title);
        ValidateDescription(description);

        Id = Guid.NewGuid();
        Title = title.Trim();
        Description = description?.Trim();
        IsCompleted = false;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = null;
    }

    public void Update(
        string title,
        string? description)
    {
        ValidateTitle(title);
        ValidateDescription(description);

        Title = title.Trim();
        Description = description?.Trim();
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

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(
                "Task title is required.",
                nameof(title));
        }

        if (title.Trim().Length > 200)
        {
            throw new ArgumentException(
                "Task title cannot exceed 200 characters.",
                nameof(title));
        }
    }

    private static void ValidateDescription(string? description)
    {
        if (description is not null &&
            description.Trim().Length > 2000)
        {
            throw new ArgumentException(
                "Task description cannot exceed 2000 characters.",
                nameof(description));
        }
    }
}