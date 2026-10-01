using Microsoft.AspNetCore.Mvc;
using TaskApi.Requests;
using TaskApi.Domain;

namespace TaskApi.Controllers;

/// <summary>
/// Provides read-only HTTP endpoints for task resources.
/// </summary>

[ApiController]
[Route("api/[controller]")]
public sealed class TasksController: ControllerBase
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

    /// <summary>
    /// Creates a new task.
    /// </summary>
    /// <param name="request">The task creation data.</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<TaskItem> Create(TaskWriteRequest  request)
    {
        try
        {
            var task = new TaskItem(
                request.Title,
                request.Description);
            Tasks.Add(task);
            return CreatedAtAction(
                nameof(GetById),
                new { id = task.Id },
                task);
        }
        catch (ArgumentException exception)
        {
            return BadRequest( new 
            {
                message = exception.Message,
                parameter = exception.ParamName   
            });
        }
    }

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <param name="id">The unique identifier of the task.</param>
    /// <param name="request">The updated task data.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Update(Guid id, TaskWriteRequest request)
    {
        var task = Tasks.FirstOrDefault(item => item.Id == id);
        if (task is null)
        {
            return NotFound();
        }


        try
        {
            task.Update(
                request.Title,
                request.Description);
        }
        catch (ArgumentException exception)
        {
            return BadRequest( new
            {
                message = exception.Message,
                parameter = exception.ParamName
                    
            });
        }
        return NoContent();
    }

    /// <summary>
    /// Deletes an existing task.
    /// </summary>
    /// <param name="id">The unique identifier of the task.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        var task = Tasks.FirstOrDefault(item => item.Id == id);
        if (task is null)
        {
            return NotFound();
        }
        Tasks.Remove(task);
        return NoContent();
    }
}