using System.ComponentModel.DataAnnotations;

namespace TaskApi.DTOs;

/// <summary>
/// Defines filtering, searching, sorting, and pagination options for tasks.
/// </summary>
public sealed class TaskQuery : IValidatableObject
{
    private static readonly HashSet<string> SupportedSorts = new(StringComparer.OrdinalIgnoreCase)
    {
        "title",
        "title_desc",
        "createdat",
        "createdat_desc",
        "created_at",
        "created_at_desc",
        "completed",
        "completed_desc"
    };

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

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Page < 1)
        {
            yield return new ValidationResult(
                "Page must be greater than or equal to 1.",
                new[] { nameof(Page) });
        }

        if (PageSize < 1 || PageSize > 100)
        {
            yield return new ValidationResult(
                "PageSize must be between 1 and 100.",
                new[] { nameof(PageSize) });
        }

        if (!string.IsNullOrWhiteSpace(Sort) &&
            !SupportedSorts.Contains(Sort.Trim()))
        {
            yield return new ValidationResult(
                "Sort must be one of: title, title_desc, createdAt, createdAt_desc, created_at, created_at_desc, completed, completed_desc.",
                new[] { nameof(Sort) });
        }
    }
}