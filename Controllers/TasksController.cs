using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskApi.DTOs;
using TaskApi.Services;

namespace TaskApi.Controllers;

/// <summary>
/// Provides HTTP endpoints for managing task resources.
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class TasksController(
    ITaskService taskService) : ControllerBase
{
    /// <summary>
    /// Returns a filtered, sorted, and paginated collection of tasks.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedTaskResponse>> GetAll(
        [FromQuery] TaskQuery query)
    {
        var result = await taskService.GetAllAsync(query);

        return Ok(
            PagedTaskResponse.FromDomain(result));
    }

    /// <summary>
    /// Returns a task by its unique identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> GetById(Guid id)
    {
        var task = await taskService.GetByIdAsync(id);

        if (task is null)
        {
            return NotFound();
        }

        return Ok(
            TaskResponse.FromDomain(task));
    }

    /// <summary>
    /// Creates a new task.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskResponse>> Create(
        CreateTaskRequest request)
    {
        var task = await taskService.CreateAsync(
            request.Title,
            request.Description);

        var response =
            TaskResponse.FromDomain(task);

        return CreatedAtAction(
            nameof(GetById),
            new { id = task.Id },
            response);
    }

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateTaskRequest request)
    {
        var updated =
            await taskService.UpdateAsync(
                id,
                request.Title,
                request.Description);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes an existing task.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted =
            await taskService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}