using System.ComponentModel.DataAnnotations;

namespace TaskApi.Requests;

/// <summary>
/// Represents the data required to create a task.
/// </summary>
public sealed class CreateTaskRequest
{
    [Required]
    [StringLength(
        200,
        MinimumLength = 1,
        ErrorMessage = "Title must be between 1 and 200 characters.")]
    public string Title { get; init; } = string.Empty;
    
    [StringLength(
        2000,
        ErrorMessage = "Description can not exceed 2000 characters.")]
    public string? Description { get; set; }
}