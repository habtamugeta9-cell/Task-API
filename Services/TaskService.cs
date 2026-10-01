using TaskApi.Domain;

namespace TaskApi.Services;


/// <summary>
/// Provides application operations for task resources using in-memory storage.
/// </summary>
public sealed class TaskService : ITaskService
{
    private readonly List<TaskItem> _tasks =
    [
        new(
            "Learn ASP.NET Core",
            "Build the first REST API."
        ),
        new(
            "Build Task API",
            "Practice controllers and HTTP endpoints."
        ),
        new(
            "Practice C# LINQ",
            "Learn how to query in-memory collections."
        )
    ];
    
    private readonly object _lock = new();
    
        /// <inheritdoc />
    public IReadOnlyList<TaskItem> GetAll()
    {
        lock (_lock)
        {
            return _items.ToList();
        }
    }

    /// <inheritdoc />
    public TaskItem? GetById(Guid id)
    {
        lock (_lock)
        {
            return _items.FirstOrDefault(x => x.Id == id);
        }
    }

    /// <inheritdoc />
    public TaskItem Create(string title, string? description)
    {
        var task = new TaskItem(title, description);

        lock (_lock)
        {
            _tasks.Add(task);
        }
        return task;
    }

    /// <inheritdoc />
    public bool Update(Guid id, string title, string? description)
    {
        var task = _tasks.FirstOrDefault(item => item.Id == id);
        if (task is null)
        {
            return false;
        }
        task.Update(title, description);

        return true;
    }

    /// <inheritdoc />
    public bool Delete(Guid id)
    {
        lock (_lock)
        {
            var task = _tasks.FirstOrDefault(item => item.Id == id);
            if (task is null)
            {
                return false;
            }
            return _tasks.Remove(task);
        }
    }
}

