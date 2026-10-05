namespace TaskApi.Domain;

public sealed class TaskItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid UserId { get; private set; }

    public string Title { get; private set; }

    public string? Description { get; private set; }

    public bool IsCompleted { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; } =
        DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; private set; }

    private TaskItem()
    {
        Title = string.Empty;
    }

    public TaskItem(
        Guid userId,
        string title,
        string? description)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "User ID is required.",
                nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(
                "Title is required.",
                nameof(title));
        }

        if (title.Length > 200)
        {
            throw new ArgumentException(
                "Title cannot exceed 200 characters.",
                nameof(title));
        }

        if (description?.Length > 2000)
        {
            throw new ArgumentException(
                "Description cannot exceed 2000 characters.",
                nameof(description));
        }

        UserId = userId;
        Title = title.Trim();
        Description = description?.Trim();
        IsCompleted = false;
    }

    public void Update(
        string title,
        string? description)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(
                "Title is required.",
                nameof(title));
        }

        if (title.Length > 200)
        {
            throw new ArgumentException(
                "Title cannot exceed 200 characters.",
                nameof(title));
        }

        if (description?.Length > 2000)
        {
            throw new ArgumentException(
                "Description cannot exceed 2000 characters.",
                nameof(description));
        }

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
}