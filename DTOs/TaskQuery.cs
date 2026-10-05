namespace TaskApi.DTOs;

/// <summary>
/// Defines filtering, searching, sorting, and pagination options for tasks.
/// </summary>
public sealed class TaskQuery
{
    /// <summary>
    /// Searches task titles and descriptions.
    /// </summary>
    public string? Search { get; init; }

    /// <summary>
    /// Filters tasks by completion state.
    /// </summary>
    public bool? Completed { get; init; }

    /// <summary>
    /// Sorts the task collection.
    /// Supported values:
    /// title, title_desc, createdAt, createdAt_desc,
    /// completed, completed_desc.
    /// </summary>
    public string? Sort { get; init; }

    /// <summary>
    /// The page number. Starts at 1.
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// The maximum number of results per page.
    /// </summary>
    public int PageSize { get; init; } = 20;
}