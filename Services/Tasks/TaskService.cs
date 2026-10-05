using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.Domain;
using TaskApi.DTOs;

namespace TaskApi.Services.Tasks;

public sealed class TaskService(
    AppDbContext dbContext,
    ILogger<TaskService> logger,
    ITaskQueryBuilder queryBuilder) : ITaskService
{
    public async Task<PagedResult<TaskItem>> GetAllAsync(
        TaskQuery query,
        Guid userId,
        bool isAdmin)
    {
        var tasks = dbContext.Tasks
            .AsNoTracking();

        if (!isAdmin)
        {
            tasks = tasks.Where(
                task => task.UserId == userId);
        }

        tasks = queryBuilder.ApplyFilters(
            tasks,
            query);

        var page =
            queryBuilder.NormalizePage(query.Page);

        var pageSize =
            queryBuilder.NormalizePageSize(query.PageSize);

        var totalCount =
            await tasks.CountAsync();

        var totalPages =
            totalCount == 0
                ? 0
                : (int)Math.Ceiling(
                    totalCount / (double)pageSize);

        var items =
            await tasks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        logger.LogDebug(
            "Retrieved tasks for UserId: {UserId}. " +
            "IsAdmin: {IsAdmin}, Page: {Page}, " +
            "PageSize: {PageSize}, TotalCount: {TotalCount}",
            userId,
            isAdmin,
            page,
            pageSize,
            totalCount);

        return new PagedResult<TaskItem>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }

    public async Task<TaskItem?> GetByIdAsync(
        Guid id,
        Guid userId,
        bool isAdmin)
    {
        var query = dbContext.Tasks
            .AsNoTracking()
            .Where(task => task.Id == id);

        if (!isAdmin)
        {
            query = query.Where(
                task => task.UserId == userId);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<TaskItem> CreateAsync(
        Guid userId,
        string title,
        string? description)
    {
        var task = new TaskItem(
            userId,
            title,
            description);

        dbContext.Tasks.Add(task);

        await dbContext.SaveChangesAsync();

        logger.LogInformation(
            "Task created. TaskId: {TaskId}, UserId: {UserId}",
            task.Id,
            userId);

        return task;
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        Guid userId,
        bool isAdmin,
        string title,
        string? description)
    {
        var query = dbContext.Tasks
            .Where(task => task.Id == id);

        if (!isAdmin)
        {
            query = query.Where(
                task => task.UserId == userId);
        }

        var task =
            await query.FirstOrDefaultAsync();

        if (task is null)
        {
            logger.LogWarning(
                "Task update rejected. " +
                "TaskId: {TaskId}, UserId: {UserId}, IsAdmin: {IsAdmin}",
                id,
                userId,
                isAdmin);

            return false;
        }

        task.Update(
            title,
            description);

        await dbContext.SaveChangesAsync();

        logger.LogInformation(
            "Task updated. TaskId: {TaskId}, UserId: {UserId}",
            id,
            userId);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        Guid userId,
        bool isAdmin)
    {
        var query = dbContext.Tasks
            .Where(task => task.Id == id);

        if (!isAdmin)
        {
            query = query.Where(
                task => task.UserId == userId);
        }

        var task =
            await query.FirstOrDefaultAsync();

        if (task is null)
        {
            logger.LogWarning(
                "Task deletion rejected. " +
                "TaskId: {TaskId}, UserId: {UserId}, IsAdmin: {IsAdmin}",
                id,
                userId,
                isAdmin);

            return false;
        }

        dbContext.Tasks.Remove(task);

        await dbContext.SaveChangesAsync();

        logger.LogInformation(
            "Task deleted. TaskId: {TaskId}, UserId: {UserId}",
            id,
            userId);

        return true;
    }
}