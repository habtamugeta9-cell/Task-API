

using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.Domain;

namespace TaskApi.Services;

/// <summary>
/// Provides application operations for task resources using PostgreSQL.
/// </summary>
public sealed class TaskService(
    AppDbContext dbContext): ITaskService
{
    public async Task<IReadOnlyList<TaskItem>> GetAllAsync()
    {
        return await dbContext
            .Tasks
            .AsNoTracking()
            .OrderByDescending(task => task.CreatedAt)
            .ToListAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        var task = await dbContext
            .Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(task => task.Id == id);
        return task;
    }

    public async Task<TaskItem?> CreateAsync(TaskItem task)
    {
        var createTask = new TaskItem(task.Title, task.Description);
        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync();
        return createTask;
    }

    public async Task<bool> UpdateAsync(Guid id, string title, string? description)
    {
        var task = await dbContext.Tasks.FirstOrDefaultAsync(task => task.Id == id);
        if (task is null)
        {
            return false;
        }
        task.Update(title, description);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var task = await dbContext.Tasks
            .FirstOrDefaultAsync(item => item.Id == id);
        if (task is null)
        {
            return false;
        }

        dbContext.Tasks.Remove(task);
        await dbContext.SaveChangesAsync();

        return true;
    }
}