using TaskApi.Domain;
using TaskApi.DTOs;

namespace TaskApi.Services.Tasks;

public interface ITaskService
{
    Task<PagedResult<TaskItem>> GetAllAsync(
        TaskQuery query,
        Guid userId,
        bool isAdmin);

    Task<TaskItem?> GetByIdAsync(
        Guid id,
        Guid userId,
        bool isAdmin);

    Task<TaskItem> CreateAsync(
        Guid userId,
        string title,
        string? description);

    Task<TaskMutationResult> UpdateAsync(
        Guid id,
        Guid userId,
        bool isAdmin,
        string title,
        string? description,
        Guid expectedVersion);

    Task<TaskMutationResult> SetCompletionAsync(
        Guid id,
        Guid userId,
        bool isAdmin,
        bool completed,
        Guid expectedVersion);

    Task<TaskMutationResult> DeleteAsync(
        Guid id,
        Guid userId,
        bool isAdmin,
        Guid expectedVersion);

    Task<bool> UpdateAsync(
        Guid id,
        Guid userId,
        bool isAdmin,
        string title,
        string? description);

    Task<bool> DeleteAsync(
        Guid id,
        Guid userId,
        bool isAdmin);
}