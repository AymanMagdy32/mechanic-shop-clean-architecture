using MechanicShop.Application.Features.WorkOrders.Commands.AssignLapor;
using MechanicShop.Application.Features.WorkOrders.Commands.CreateWorkOrders;
using MechanicShop.Application.Features.WorkOrders.Commands.RelocateWorkOrders;
using MechanicShop.Application.Features.WorkOrders.Commands.RemoveWorkOrders;
using MechanicShop.Application.Features.WorkOrders.Commands.UpdateOrderState;
using MechanicShop.Application.Features.WorkOrders.Commands.UpdateWorkOrdersRepairTasks;

using MechanicShop.Application.Features.WorkOrders.Dtos;
using MechanicShop.Application.Features.WorkOrders.Queries.GetWorkOrderByIdQuery;
using MechanicShop.Application.Features.WorkOrders.Queries.GetWorkOrders;

using MechanicShop.Domain.Entities.WorkOrders.Enums;

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MechanicShop.Api.Controllers;

[ApiController]
[Route("api/work-orders")]
public sealed class WorkOrdersController(ISender sender) : ControllerBase
{
    // ============================================================
    // GET ALL WORK ORDERS
    // GET: api/work-orders
    // ============================================================

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetWorkOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string sortColumn = "createdAt",
        [FromQuery] string sortDirection = "desc",
        [FromQuery] WorkOrderState? state = null,
        [FromQuery] Guid? vehicleId = null,
        [FromQuery] Guid? laborId = null,
        [FromQuery] DateTime? startDateFrom = null,
        [FromQuery] DateTime? startDateTo = null,
        [FromQuery] DateTime? endDateFrom = null,
        [FromQuery] DateTime? endDateTo = null,
        [FromQuery] Spot? spot = null,
        CancellationToken ct = default)
    {
        if (page <= 0)
        {
            return BadRequest(new
            {
                message = "Page must be greater than 0."
            });
        }

        if (pageSize <= 0)
        {
            return BadRequest(new
            {
                message = "PageSize must be greater than 0."
            });
        }

        var query = new GetWorkOrdersQuery(
            Page: page,
            PageSize: pageSize,
            SearchTerm: searchTerm,
            SortColumn: sortColumn,
            SortDirection: sortDirection,
            State: state,
            VehicleId: vehicleId,
            LaborId: laborId,
            StartDateFrom: startDateFrom,
            StartDateTo: startDateTo,
            EndDateFrom: endDateFrom,
            EndDateTo: endDateTo,
            Spot: spot);

        var result = await sender.Send(query, ct);

        if (result.IsError)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Value);
    }


    // ============================================================
    // GET WORK ORDER BY ID
    // GET: api/work-orders/{workOrderId}
    // ============================================================

    [HttpGet("{workOrderId:guid}")]
    [ProducesResponseType(typeof(WorkOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWorkOrderById(
        Guid workOrderId,
        CancellationToken ct)
    {
        var query = new GetWorkOrderByIdQuery(workOrderId);

        var result = await sender.Send(query, ct);

        if (result.IsError)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }


    // ============================================================
    // CREATE WORK ORDER
    // POST: api/work-orders
    // ============================================================

    [HttpPost]
    [ProducesResponseType(typeof(WorkOrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateWorkOrder(
        [FromBody] CreateWorkOrderCommand command,
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


    // ============================================================
    // ASSIGN LABOR
    // PUT: api/work-orders/{workOrderId}/labor
    // ============================================================

    [HttpPut("{workOrderId:guid}/labor")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AssignLabor(
        Guid workOrderId,
        [FromBody] AssignLaborCommand command,
        CancellationToken ct)
    {
        if (workOrderId != command.WorkOrderId)
        {
            return BadRequest(new
            {
                message =
                    "WorkOrder ID in route does not match WorkOrder ID in request body."
            });
        }

        var result = await sender.Send(command, ct);

        if (result.IsError)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }


    // ============================================================
    // RELOCATE WORK ORDER
    // PUT: api/work-orders/{workOrderId}/relocate
    // ============================================================

    [HttpPut("{workOrderId:guid}/relocate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RelocateWorkOrder(
        Guid workOrderId,
        [FromBody] RelocateWorkOrdersCommand command,
        CancellationToken ct)
    {
        if (workOrderId != command.WorkOrderId)
        {
            return BadRequest(new
            {
                message =
                    "WorkOrder ID in route does not match WorkOrder ID in request body."
            });
        }

        var result = await sender.Send(command, ct);

        if (result.IsError)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }


    // ============================================================
    // UPDATE WORK ORDER STATE
    // PUT: api/work-orders/{workOrderId}/state
    // ============================================================

    [HttpPut("{workOrderId:guid}/state")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateWorkOrderState(
        Guid workOrderId,
        [FromBody] UpdateOrderStateCommand command,
        CancellationToken ct)
    {
        if (workOrderId != command.WorkOrderId)
        {
            return BadRequest(new
            {
                message =
                    "WorkOrder ID in route does not match WorkOrder ID in request body."
            });
        }

        var result = await sender.Send(command, ct);

        if (result.IsError)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }


    // ============================================================
    // UPDATE REPAIR TASKS
    // PUT: api/work-orders/{workOrderId}/repair-tasks
    // ============================================================

    [HttpPut("{workOrderId:guid}/repair-tasks")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateRepairTasks(
        Guid workOrderId,
        [FromBody] UpdateWorkOrdersRepairTasksCommand command,
        CancellationToken ct)
    {
        if (workOrderId != command.WorkOrderId)
        {
            return BadRequest(new
            {
                message =
                    "WorkOrder ID in route does not match WorkOrder ID in request body."
            });
        }

        var result = await sender.Send(command, ct);

        if (result.IsError)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }


    // ============================================================
    // DELETE WORK ORDER
    // DELETE: api/work-orders/{workOrderId}
    // ============================================================

    [HttpDelete("{workOrderId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteWorkOrder(
        Guid workOrderId,
        CancellationToken ct)
    {
        var command = new RemoveWorkOrdersCommand(workOrderId);

        var result = await sender.Send(command, ct);

        if (result.IsError)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }
}