using Microsoft.AspNetCore.Mvc;

namespace TaskApi.Controllers;

using TaskAPi.Domain;

/// <summary>
/// Provides read-only HTTP endpoints for task resources.
/// </summary>e

[ApiController]
[Route("api/[controller]")]
public sealed class TaskController: ControllerBase
{
    private static readonly List<TaskItem> Tasks =
    [
        new(
            "Learn ASP.NET Core",
            "Build the first REST API."
        ),
        new(
            "Build Task API",
            "Practice controllers and HTTP endpoints."
        ),
        new(
            "Practice C# LINQ",
            "Learn how to query in-memory collections."
        )
    ];


    /// <summary>
    /// Returns all tasks.
    /// </summary>
    /// <returns>A collection of tasks.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<TaskItem>> GetAll()
    {
        return Ok(Tasks);
    }
    
    /// <summary>
    /// Returns a task by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the task.</param>
    /// <returns>The requested task if it exists.</returns>
    [HttpGet("{id:guid}")]

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TaskItem> GetById(Guid id)
    {
        var task = Tasks.FirstOrDefault(item => item.Id == id);

        if (task is null)
        {
            return NotFound();
        }

        return Ok(task);
    }
    
    

}