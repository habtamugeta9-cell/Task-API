using TaskApi.Domain;

namespace TaskApi.DTOs;

/// <summary>
/// Represents a paginated task API response.
/// </summary>
public sealed class PagedTaskResponse
{
    public IReadOnlyList<TaskResponse> Items { get; init; } = [];

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    public int TotalPages { get; init; }

    public bool HasNextPage { get; init; }

    public bool HasPreviousPage { get; init; }

    public static PagedTaskResponse FromDomain(
        PagedResult<TaskItem> result)
    {
        return new PagedTaskResponse
        {
            Items = result.Items
                .Select(TaskResponse.FromDomain)
                .ToList(),

            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages,
            HasNextPage = result.HasNextPage,
            HasPreviousPage = result.HasPreviousPage
        };
    }
}