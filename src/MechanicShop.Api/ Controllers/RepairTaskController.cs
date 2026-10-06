using MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask;
using MechanicShop.Application.Features.RepairTasks.Commands.RemoveRepairTask;
using MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask;
using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTaskById;
using MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTasks;

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MechanicShop.Api.Controllers;

[ApiController]
[Route("api/repair-tasks")]
public sealed class RepairTasksController(ISender sender) : ControllerBase
{
    // GET: api/repair-tasks
    [HttpGet]
    [ProducesResponseType(typeof(List<RepairTaskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRepairTasks(
        CancellationToken ct)
    {
        var result = await sender.Send(
            new GetRepairTasksQuery(),
            ct);

        if (result.IsError)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }


    // GET: api/repair-tasks/{repairTaskId}
    [HttpGet("{repairTaskId:guid}")]
    [ProducesResponseType(typeof(RepairTaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRepairTaskById(
        Guid repairTaskId,
        CancellationToken ct)
    {
        var query = new GetRepairTaskByIdQuery(repairTaskId);

        var result = await sender.Send(query, ct);

        if (result.IsError)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }


    // POST: api/repair-tasks
    [HttpPost]
    [ProducesResponseType(typeof(RepairTaskDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateRepairTask(
        [FromBody] CreateRepairTaskCommand command,
        CancellationToken ct)
    {
        var result = await sender.Send(command, ct);

        if (result.IsError)
        {
            return BadRequest(result.Errors);
        }

        return StatusCode(
            StatusCodes.Status201Created,
            result.Value);
    }


    // PUT: api/repair-tasks/{repairTaskId}
    [HttpPut("{repairTaskId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRepairTask(
        Guid repairTaskId,
        [FromBody] UpdateRepairTaskCommand command,
        CancellationToken ct)
    {
        if (repairTaskId != command.RepairTaskId)
        {
            return BadRequest(new
            {
                message =
                    "RepairTask ID in route does not match RepairTask ID in request body."
            });
        }

        var result = await sender.Send(command, ct);

        if (result.IsError)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }


    // DELETE: api/repair-tasks/{repairTaskId}
    [HttpDelete("{repairTaskId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteRepairTask(
        Guid repairTaskId,
        CancellationToken ct)
    {
        var command = new RemoveRepairTaskCommand(repairTaskId);

        var result = await sender.Send(command, ct);

        if (result.IsError)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }
}