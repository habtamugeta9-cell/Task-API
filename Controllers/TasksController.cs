using Microsoft.AspNetCore.Mvc;
using TaskApi.Requests;
using TaskApi.Responses;
using TaskApi.Services;

namespace TaskApi.Controllers;
/// <summary>
/// Provides HTTP endpoints for managing task resources.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class TaskController(
    ITaskService taskService) : ControllerBase
{

    /// <summary>
    /// Returns all tasks.
    /// </summary>
    /// <returns>A collection of task responses.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<IEnumerable<TaskResponse>> GetTasks()
    {
        var tasks = taskService.GetAll();
        
        var response = tasks
            .Select(TaskResponse.FromDomain)
            .ToList();
        return Ok(response);
    }

    /// <summary>
    /// Returns a task by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the task.</param>
    /// <returns>The requested task.</returns>

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<TaskResponse> GetTaskById(Guid id)
    {
        var task = taskService.GetById(id);

        if (task == null)
        {
            return NotFound();
        }
        return Ok(TaskResponse.FromDomain(task));
    }

    /// <summary>
    /// Creates a new task.
    /// </summary>
    /// <param name="request">The task creation data.</param>
    /// <returns>The newly created task.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<TaskResponse> CreateTask(CreateTaskRequest request)
    {
        try
        {
            var task = taskService.Create(
                request.Title,
                request.Description);
            var response = TaskResponse.FromDomain(task);
            
            return CreatedAtAction(
                nameof(GetTaskById), 
                new { id = task.Id }, 
                response);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = exception.Message,
                parameters = exception.ParamName

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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateTask(Guid id, UpdateTaskRequest request)
    {
        var existingTask = taskService.GetById(id);
        if (existingTask is null)
        {
            return NotFound();
        }

        try
        {
            taskService.Update(
                id,
                request.Title,
                request.Description);
            return NoContent();
        }
        catch (ArgumentException e)
        {
            return BadRequest(
                new
                {
                    message = e.Message,
                    parameters = e.ParamName
                });
        }
    }

    /// <summary>
    /// Deletes an existing task.
    /// </summary>
    /// <param name="id">The unique identifier of the task.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult DeleteTask(Guid id)
    {
        var task = taskService.Delete(id);
        if (!task)
        {
            return NotFound();
        }
        return NoContent();
    }
}