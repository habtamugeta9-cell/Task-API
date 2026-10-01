using TaskApi.Domain;


namespace TaskApi.Services;


/// <summary>
/// Provides application operations for task resources using in-memory storage.
/// </summary>
public sealed class TaskService : ITaskService
{
    private readonly List<TaskItem> _taskItems =
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

    public IReadOnlyList<TaskItem> GetAll()
    {
        lock (_lock)
        {
            return _taskItems.ToList();
            
        }
    }

    public TaskItem? GetById(Guid id)
    {
        lock (_lock)
        {
            return _taskItems.FirstOrDefault(taskItem => taskItem.Id == id);
        }
    }

    public TaskItem Create(string title, string? description)
    {
        var taskItem = new TaskItem(title, description);
        lock (_lock)
        {
            _taskItems.Add(taskItem);
        }
        return taskItem;
    }

    public bool Update(Guid id, string title, string? description)
    {
        lock (_lock)
        {
            var task = _taskItems.FirstOrDefault(item => item.Id == id);
            if (task is null)
            {
                return false;
            }
            task.Update(title, description);
            return true;
        }
    }

    public bool Delete(Guid id)
    {
        lock (_lock)
        {
           var tak = _taskItems.FirstOrDefault(item => item.Id == id);
           if (tak is null)
           {
               return false;
           }
           return _taskItems.Remove(tak);
        }
    }
    
}
