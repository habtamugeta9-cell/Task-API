using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TaskApi.Authorization;
using TaskApi.DTOs;
using TaskApi.Services;
using TaskApi.Services.Tasks;

namespace TaskApi.Controllers;

[EnableRateLimiting("api")]
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

        Response.Headers.ETag =
            $"\"{task.Version}\"";

        return Ok(
            TaskResponse.FromDomain(task));
    }

    [HttpPatch("{id:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(StatusCodes.Status428PreconditionRequired)]
    public Task<IActionResult> Complete(
        Guid id)
    {
        return SetCompletion(id, true);
    }

    [HttpPatch("{id:guid}/uncomplete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(StatusCodes.Status428PreconditionRequired)]
    public Task<IActionResult> Uncomplete(
        Guid id)
    {
        return SetCompletion(id, false);
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
    [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateTaskRequest request)
    {
        if (!TryGetExpectedVersion(
                out var expectedVersion))
        {
            return StatusCode(
                StatusCodes.Status428PreconditionRequired);
        }

        var result =
            await taskService.UpdateAsync(
                id,
                currentUser.UserId,
                currentUser.IsAdmin,
                request.Title,
                request.Description,
                expectedVersion);

        return result.Status switch
        {
            TaskMutationStatus.Success => NoContent(),
            TaskMutationStatus.NotFound => NotFound(),
            TaskMutationStatus.Conflict => StatusCode(
                StatusCodes.Status412PreconditionFailed),
            _ => StatusCode(
                StatusCodes.Status500InternalServerError)
        };
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> Delete(
        Guid id)
    {
        if (!TryGetExpectedVersion(
                out var expectedVersion))
        {
            return StatusCode(
                StatusCodes.Status428PreconditionRequired);
        }

        var result =
            await taskService.DeleteAsync(
                id,
                currentUser.UserId,
                currentUser.IsAdmin,
                expectedVersion);

        return result.Status switch
        {
            TaskMutationStatus.Success => NoContent(),
            TaskMutationStatus.NotFound => NotFound(),
            TaskMutationStatus.Conflict => StatusCode(
                StatusCodes.Status412PreconditionFailed),
            _ => StatusCode(
                StatusCodes.Status500InternalServerError)
        };
    }

    private async Task<IActionResult> SetCompletion(
        Guid id,
        bool completed)
    {
        if (!TryGetExpectedVersion(
                out var expectedVersion))
        {
            return StatusCode(
                StatusCodes.Status428PreconditionRequired);
        }

        var result =
            await taskService.SetCompletionAsync(
                id,
                currentUser.UserId,
                currentUser.IsAdmin,
                completed,
                expectedVersion);

        return result.Status switch
        {
            TaskMutationStatus.Success => NoContent(),
            TaskMutationStatus.NotFound => NotFound(),
            TaskMutationStatus.Conflict => StatusCode(
                StatusCodes.Status412PreconditionFailed),
            _ => StatusCode(
                StatusCodes.Status500InternalServerError)
        };
    }

    private bool TryGetExpectedVersion(
        out Guid version)
    {
        version = Guid.Empty;

        var value =
            Request.Headers.IfMatch.ToString();

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        value = value.Trim();

        if (value.StartsWith('"') &&
            value.EndsWith('"'))
        {
            value = value[1..^1];
        }

        return Guid.TryParse(
            value,
            out version);
    }
}