using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.Domain;
using TaskApi.Queries;

namespace TaskApi.Services;

/// <summary>
/// Provides application operations for task resources using PostgreSQL.
/// </summary>
public sealed class TaskService(
    AppDbContext dbContext,
    ILogger<TaskService> logger) : ITaskService
{
    public async Task<PagedResult<TaskItem>> GetAllAsync(TaskQuery query)
    {
        var tasks = dbContext.Tasks.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            tasks = tasks.Where(task =>
                EF.Functions.ILike(task.Title, $"%{search}%") ||
                (task.Description != null &&
                    EF.Functions.ILike(task.Description, $"%{search}%")));
        }

        if (query.Completed.HasValue)
        {
            tasks = tasks.Where(task => task.IsCompleted == query.Completed.Value);
        }

        tasks = query.Sort?.ToLowerInvariant() switch
        {
            "title" => 
                tasks.OrderBy(task => task.Title),
            "title_desc" =>
                tasks.OrderByDescending(task => task.Title),
            "createdat" => 
                tasks.OrderBy(task => task.CreatedAt),
            "createdat_desc" => 
                tasks.OrderByDescending(task => task.CreatedAt),
            "completed" =>
                tasks.OrderBy(task => task.IsCompleted),
            "completed_desc" =>
                tasks.OrderByDescending(task => task.IsCompleted),
            _ => 
                tasks.OrderByDescending(task => task.CreatedAt)
        };

        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var totalCount = await tasks.CountAsync();
        var totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await tasks
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        logger.LogDebug(
            "Retrieved tasks. Page: {Page}, PageSize: {PageSize}, " +
            "TotalCount: {TotalCount}, SearchProvided: {SearchProvided}, " +
            "CompletedFilter: {CompletedFilter}, Sort: {Sort}",
            page,
            pageSize,
            totalCount,
            !string.IsNullOrWhiteSpace(query.Search),
            query.Completed,
            query.Sort);

        return new PagedResult<TaskItem>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
        };
    }
    
    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        return await dbContext
            .Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(task => task.Id == id);
    }

    public async Task<TaskItem> CreateAsync(string title, string? description)
    {
        var task = new TaskItem(title, description);
        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync();
        
            logger.LogInformation(
                $"Task created. Id: {task.Id}, Title: {task.Title}, Description: {task.Description}", task.Id);
        return task;
    }

    public async Task<bool> UpdateAsync(Guid id, string title, string? description)
    {
        var task = await dbContext.Tasks.FirstOrDefaultAsync(task => task.Id == id);
        if (task is null)
        {
            logger.LogWarning(
                "Task update failed because task was not found. " +
                "TaskId: {TaskId}",
                id);
        }

        task.Update(title, description);
        await dbContext.SaveChangesAsync();
        
        logger.LogInformation(
            "Task updated. TaskId: {TaskId}",
            id);
        
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var task = await dbContext.Tasks
            .FirstOrDefaultAsync(item => item.Id == id);
        if (task is null)
        {
            logger.LogWarning(
                "Task deletion failed because task was not found. " +
                "TaskId: {TaskId}",
                id);
        }

        dbContext.Tasks.Remove(task);
        await dbContext.SaveChangesAsync();
        logger.LogInformation(
            "Task deleted. TaskId: {TaskId}",
            id);
        return true;
    }
}