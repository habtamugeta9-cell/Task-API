using System.ComponentModel.DataAnnotations;

namespace TaskApi.Requests;

/// <summary>
/// Represents the data required to update a task.
/// </summary>
public sealed class UpdateTaskRequest
{
    [Required]
    [StringLength(
        200,
        MinimumLength = 1,
        ErrorMessage = "Task title must be between 1 and 200 characters.")]
    public string Title { get; init; } = string.Empty;

    [StringLength(
        2000,
        ErrorMessage = "Task description cannot exceed 2000 characters.")]
    public string? Description { get; init; }
}