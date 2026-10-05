using TaskApi.Domain;
using TaskApi.DTOs;

namespace TaskApi.Services;

public interface ITaskQueryBuilder
{
    IQueryable<TaskItem> ApplyFilters(
        IQueryable<TaskItem> tasks,
        TaskQuery query);

    IQueryable<TaskItem> ApplySearch(
        IQueryable<TaskItem> tasks,
        string? search);

    IQueryable<TaskItem> ApplyCompletedFilter(
        IQueryable<TaskItem> tasks,
        bool? completed);

    IQueryable<TaskItem> ApplySorting(
        IQueryable<TaskItem> tasks,
        string? sort);

    int NormalizePage(int page);

    int NormalizePageSize(int pageSize);
}