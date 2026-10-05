using Microsoft.EntityFrameworkCore;
using TaskApi.Domain;
using TaskApi.DTOs;

namespace TaskApi.Services.Tasks;

public sealed class TaskQueryBuilder : ITaskQueryBuilder
{
    public IQueryable<TaskItem> ApplyFilters(
        IQueryable<TaskItem> tasks,
        TaskQuery query)
    {
        tasks = ApplySearch(tasks, query.Search);
        tasks = ApplyCompletedFilter(tasks, query.Completed);
        return ApplySorting(tasks, query.Sort);
    }

    public IQueryable<TaskItem> ApplySearch(
        IQueryable<TaskItem> tasks,
        string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return tasks;
        }

        var value = search.Trim();

        return tasks.Where(task =>
            EF.Functions.ILike(task.Title, $"%{value}%") ||
            (task.Description != null &&
             EF.Functions.ILike(task.Description, $"%{value}%")));
    }

    public IQueryable<TaskItem> ApplyCompletedFilter(
        IQueryable<TaskItem> tasks,
        bool? completed)
    {
        if (!completed.HasValue)
        {
            return tasks;
        }

        return tasks.Where(task => task.IsCompleted == completed.Value);
    }

    public IQueryable<TaskItem> ApplySorting(
        IQueryable<TaskItem> tasks,
        string? sort)
    {
        return sort?.ToLowerInvariant() switch
        {
            "title" => tasks.OrderBy(task => task.Title),
            "title_desc" => tasks.OrderByDescending(task => task.Title),
            "createdat" => tasks.OrderBy(task => task.CreatedAt),
            "createdat_desc" => tasks.OrderByDescending(task => task.CreatedAt),
            "created_at" => tasks.OrderBy(task => task.CreatedAt),
            "created_at_desc" => tasks.OrderByDescending(task => task.CreatedAt),
            "completed" => tasks.OrderBy(task => task.IsCompleted),
            "completed_desc" => tasks.OrderByDescending(task => task.IsCompleted),
            _ => tasks.OrderByDescending(task => task.CreatedAt)
        };
    }

    public int NormalizePage(int page)
    {
        return Math.Max(page, 1);
    }

    public int NormalizePageSize(int pageSize)
    {
        return Math.Clamp(pageSize, 1, 100);
    }
}