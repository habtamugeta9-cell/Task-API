using Microsoft.AspNetCore.Components.Web;
using TaskApi.Domain;

namespace TaskApi.Services;
/// <summary>
/// Provides application operations for task resources.
/// </summary>
public interface ITaskService
{
    Task<IReadOnlyList<TaskItem>> GetAllAsync();

    Task<TaskItem> GetByAsyncId(Guid id);

    Task<TaskItem> CreateAsync(
        string title,
        string? description);

    Task<bool> UpdateAsync(
        Guid id,
        string title,
        string? description
        );

    Task<bool> DeleteAsync(Guid id);

}