using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TaskApi.Data;
using TaskApi.Domain;
using TaskApi.DTOs;
using TaskApi.Services;

namespace TaskApi.Services.Tasks;

public sealed class TaskService : ITaskService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<TaskService> _logger;
    private readonly ITaskQueryBuilder _queryBuilder;
    private readonly TaskCacheService _cache;

    public TaskService(
        AppDbContext dbContext,
        ILogger<TaskService> logger,
        ITaskQueryBuilder queryBuilder)
        : this(
            dbContext,
            logger,
            queryBuilder,
            new TaskCacheService(
                new MemoryCache(
                    new MemoryCacheOptions())))
    {
    }

    public TaskService(
        AppDbContext dbContext,
        ILogger<TaskService> logger,
        ITaskQueryBuilder queryBuilder,
        TaskCacheService cache)
    {
        _dbContext = dbContext;
        _logger = logger;
        _queryBuilder = queryBuilder;
        _cache = cache;
    }

    private AppDbContext dbContext => _dbContext;
    private ILogger<TaskService> logger => _logger;
    private ITaskQueryBuilder queryBuilder => _queryBuilder;
    private TaskCacheService cache => _cache;
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
        if (!isAdmin &&
            cache.TryGet(userId, id, out var cachedTask))
        {
            return cachedTask;
        }

        var query = dbContext.Tasks
            .AsNoTracking()
            .Where(task => task.Id == id);

        if (!isAdmin)
        {
            query = query.Where(
                task => task.UserId == userId);
        }

        var task = await query.FirstOrDefaultAsync();

        if (task is not null && !isAdmin)
        {
            cache.Set(userId, task);
        }

        return task;
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

    public async Task<TaskMutationResult> UpdateAsync(
        Guid id,
        Guid userId,
        bool isAdmin,
        string title,
        string? description,
        Guid expectedVersion)
    {
        var query = dbContext.Tasks
            .Where(task => task.Id == id);

        if (!isAdmin)
        {
            query = query.Where(
                task => task.UserId == userId);
        }

        var task = await query.FirstOrDefaultAsync();

        if (task is null)
        {
            logger.LogWarning(
                "Task update rejected. " +
                "TaskId: {TaskId}, UserId: {UserId}, IsAdmin: {IsAdmin}",
                id,
                userId,
                isAdmin);

            return new(TaskMutationStatus.NotFound);
        }

        if (task.Version != expectedVersion)
        {
            return new(TaskMutationStatus.Conflict);
        }

        task.Update(title, description);

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            dbContext.Entry(task).State = EntityState.Detached;

            logger.LogWarning(
                "Task update concurrency conflict. " +
                "TaskId: {TaskId}, UserId: {UserId}",
                id,
                userId);

            return new(TaskMutationStatus.Conflict);
        }

        cache.Remove(userId, id);

        logger.LogInformation(
            "Task updated. TaskId: {TaskId}, UserId: {UserId}",
            id,
            userId);

        return new(TaskMutationStatus.Success);
    }

    public async Task<TaskMutationResult> SetCompletionAsync(
        Guid id,
        Guid userId,
        bool isAdmin,
        bool completed,
        Guid expectedVersion)
    {
        var query = dbContext.Tasks
            .Where(task => task.Id == id);

        if (!isAdmin)
        {
            query = query.Where(
                task => task.UserId == userId);
        }

        var task = await query.FirstOrDefaultAsync();

        if (task is null)
        {
            return new(TaskMutationStatus.NotFound);
        }

        if (task.Version != expectedVersion)
        {
            return new(TaskMutationStatus.Conflict);
        }

        if (completed)
        {
            task.Complete();
        }
        else
        {
            task.Uncomplete();
        }

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            dbContext.Entry(task).State = EntityState.Detached;

            logger.LogWarning(
                "Task completion concurrency conflict. " +
                "TaskId: {TaskId}, UserId: {UserId}",
                id,
                userId);

            return new(TaskMutationStatus.Conflict);
        }

        cache.Remove(userId, id);

        return new(TaskMutationStatus.Success);
    }

    public async Task<TaskMutationResult> DeleteAsync(
        Guid id,
        Guid userId,
        bool isAdmin,
        Guid expectedVersion)
    {
        var query = dbContext.Tasks
            .Where(task => task.Id == id);

        if (!isAdmin)
        {
            query = query.Where(
                task => task.UserId == userId);
        }

        var task = await query.FirstOrDefaultAsync();

        if (task is null)
        {
            logger.LogWarning(
                "Task deletion rejected. " +
                "TaskId: {TaskId}, UserId: {UserId}, IsAdmin: {IsAdmin}",
                id,
                userId,
                isAdmin);

            return new(TaskMutationStatus.NotFound);
        }

        if (task.Version != expectedVersion)
        {
            return new(TaskMutationStatus.Conflict);
        }

        dbContext.Tasks.Remove(task);

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            logger.LogWarning(
                "Task deletion concurrency conflict. " +
                "TaskId: {TaskId}, UserId: {UserId}",
                id,
                userId);

            return new(TaskMutationStatus.Conflict);
        }

        cache.Remove(userId, id);

        logger.LogInformation(
            "Task deleted. TaskId: {TaskId}, UserId: {UserId}",
            id,
            userId);

        return new(TaskMutationStatus.Success);
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

        var task = await query.FirstOrDefaultAsync();

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

        task.Update(title, description);

        await dbContext.SaveChangesAsync();

        cache.Remove(userId, id);

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

        var task = await query.FirstOrDefaultAsync();

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

        cache.Remove(userId, id);

        logger.LogInformation(
            "Task deleted. TaskId: {TaskId}, UserId: {UserId}",
            id,
            userId);

        return true;
    }
}