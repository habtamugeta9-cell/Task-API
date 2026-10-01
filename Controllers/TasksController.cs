using Microsoft.AspNetCore.Mvc;
using TaskApi.Domain;
using TaskApi.Requests;
using TaskApi.Services;

namespace TaskApi.Controllers;

/// <summary>
/// Provides HTTP endpoints for managing task resources.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    /// <summary>
    /// Returns all tasks.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<TaskItem>> GetAll()
    {
        return Ok(_taskService.GetAll());
    }

    /// <summary>
    /// Returns a task by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the task.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TaskItem> GetById(Guid id)
    {
        var task = _taskService.GetById(id);

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
    public ActionResult<TaskItem> Create(TaskWriteRequest request)
    {
        try
        {
            var task = _taskService.Create(
                request.Title,
                request.Description);

            return CreatedAtAction(
                nameof(GetById),
                new { id = task.Id },
                task);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = exception.Message,
                parameter = exception.ParamName
            });
        }
    }

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <param name="request">The updated task data.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(
        Guid id,
        TaskWriteRequest request)
    {
        var existingTask = _taskService.GetById(id);

        if (existingTask is null)
        {
            return NotFound();
        }

        try
        {
            _taskService.Update(
                id,
                request.Title,
                request.Description);

            return NoContent();
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = exception.Message,
                parameter = exception.ParamName
            });
        }
    }

    /// <summary>
    /// Deletes an existing task.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        var deleted = _taskService.Delete(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}