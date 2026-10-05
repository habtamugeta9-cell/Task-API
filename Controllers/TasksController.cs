using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskApi.Authorization;
using TaskApi.DTOs;
using TaskApi.Services;
using TaskApi.Services.Tasks;

namespace TaskApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class TasksController(
    ITaskService taskService,
    ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedTaskResponse>> GetAll(
        [FromQuery] TaskQuery query)
    {
        var result =
            await taskService.GetAllAsync(
                query,
                currentUser.UserId,
                currentUser.IsAdmin);

        return Ok(
            PagedTaskResponse.FromDomain(result));
    }

    [HttpGet("all")]
    [Authorize(Policy = Policies.AdminOnly)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedTaskResponse>> GetAllTasks(
        [FromQuery] TaskQuery query)
    {
        var result =
            await taskService.GetAllAsync(
                query,
                currentUser.UserId,
                true);

        return Ok(
            PagedTaskResponse.FromDomain(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> GetById(
        Guid id)
    {
        var task =
            await taskService.GetByIdAsync(
                id,
                currentUser.UserId,
                currentUser.IsAdmin);

        if (task is null)
        {
            return NotFound();
        }

        return Ok(
            TaskResponse.FromDomain(task));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskResponse>> Create(
        CreateTaskRequest request)
    {
        var task =
            await taskService.CreateAsync(
                currentUser.UserId,
                request.Title,
                request.Description);

        var response =
            TaskResponse.FromDomain(task);

        return CreatedAtAction(
            nameof(GetById),
            new { id = task.Id },
            response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateTaskRequest request)
    {
        var updated =
            await taskService.UpdateAsync(
                id,
                currentUser.UserId,
                currentUser.IsAdmin,
                request.Title,
                request.Description);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id)
    {
        var deleted =
            await taskService.DeleteAsync(
                id,
                currentUser.UserId,
                currentUser.IsAdmin);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}