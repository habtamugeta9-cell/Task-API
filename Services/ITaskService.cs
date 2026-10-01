using TaskApi.Domain;

namespace TaskApi.Services;

/// <summary>
/// Provides application operations for task resources.
/// </summary>
public interface ITaskService
{
    IReadOnlyList<TaskItem> GetAll();

    TaskItem? GetById(Guid id);

    TaskItem Create(string title, string? description);

    bool Update(Guid id, string title, string? description);

    bool Delete(Guid id);
}