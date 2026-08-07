using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders
    .Commands.UpdateWorkOrdersRepairTasks;

public sealed class UpdateWorkOrdersRepairTasksHandler(
    IAppDbContext context,
    ILogger<UpdateWorkOrdersRepairTasksHandler> logger)
    : IRequestHandler<
        UpdateWorkOrdersRepairTasksCommand,
        Result<Updated>>
{
    public async Task<Result<Updated>> Handle(
        UpdateWorkOrdersRepairTasksCommand request,
        CancellationToken cancellationToken)
    {
        var requestedRepairTaskIds = request.RepairTasksIds
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToArray();

        if (requestedRepairTaskIds.Length == 0)
        {
            return Error.Validation(
                code: "WorkOrder.RepairTasks.Required",
                message: "At least one repair task is required.");
        }

        var workOrder = await context.WorkOrders
            .Include(workOrder => workOrder.RepairTasks)
            .FirstOrDefaultAsync(
                workOrder => workOrder.Id == request.WorkOrderId,
                cancellationToken);

        if (workOrder is null)
        {
            logger.LogWarning(
                "Work order {WorkOrderId} was not found.",
                request.WorkOrderId);

            return ApplicationErrors.WorkOrderNotFound;
        }

        var repairTasks = await context.RepairTasks
            .Where(repairTask =>
                requestedRepairTaskIds.Contains(repairTask.Id))
            .ToListAsync(cancellationToken);

        if (repairTasks.Count != requestedRepairTaskIds.Length)
        {
            logger.LogWarning(
                "One or more repair tasks were not found for work order {WorkOrderId}.",
                request.WorkOrderId);

            return ApplicationErrors.RepairTaskNotFound;
        }

        var totalDurationInMinutes = repairTasks.Sum(
            repairTask =>
                Convert.ToDouble(
                    repairTask.EstimatedDurationInMins));

        if (totalDurationInMinutes <= 0)
        {
            return Error.Validation(
                code: "WorkOrder.Duration.Invalid",
                message: "The total repair duration must be greater than zero.");
        }

        var startAt = workOrder.StartAtUtc;
        var endAt = startAt.AddMinutes(totalDurationInMinutes);

        var isLaborOccupied = await context.WorkOrders
            .AnyAsync(
                otherWorkOrder =>
                    otherWorkOrder.Id != workOrder.Id &&
                    otherWorkOrder.LaborId == workOrder.LaborId &&
                    otherWorkOrder.StartAtUtc < endAt &&
                    otherWorkOrder.EndAtUtc > startAt,
                cancellationToken);

        if (isLaborOccupied)
        {
            logger.LogWarning(
                "Labor {LaborId} is occupied between {StartAt} and {EndAt}.",
                workOrder.LaborId,
                startAt,
                endAt);

            return ApplicationErrors.LaborOccupied;
        }

        var hasVehicleConflict = await context.WorkOrders
            .AnyAsync(
                otherWorkOrder =>
                    otherWorkOrder.Id != workOrder.Id &&
                    otherWorkOrder.VehicleId == workOrder.VehicleId &&
                    otherWorkOrder.StartAtUtc < endAt &&
                    otherWorkOrder.EndAtUtc > startAt,
                cancellationToken);

        if (hasVehicleConflict)
        {
            logger.LogWarning(
                "Vehicle {VehicleId} has a scheduling conflict between {StartAt} and {EndAt}.",
                workOrder.VehicleId,
                startAt,
                endAt
                ) ;

            return ApplicationErrors.VehicleSchedulingConflict;
        }

        var isSpotOccupied = await context.WorkOrders
            .AnyAsync(
                    otherWorkOrder =>
                    otherWorkOrder.Id != workOrder.Id &&
                    otherWorkOrder.Spot == workOrder.Spot &&
                    otherWorkOrder.StartAtUtc < endAt &&
                    otherWorkOrder.EndAtUtc > startAt,
                cancellationToken
                 );

        if (isSpotOccupied)
        {
            logger.LogWarning(
                "Spot {Spot} is occupied between {StartAt} and {EndAt}.",
                workOrder.Spot,
                startAt,
                endAt);

            return Error.Conflict(
                code: "WorkOrder.Spot.Occupied",
                message: "The selected spot is occupied during the requested time.");
        }

        var clearResult = workOrder.ClearRepairTasks();

        if (clearResult.IsError)
        {
            logger.LogWarning(
                "Could not clear repair tasks for work order {WorkOrderId}. Errors: {@Errors}",
                workOrder.Id,
                clearResult.Errors);

            return clearResult.Errors;
        }

        foreach (var repairTask in repairTasks)
        {
            var addResult = workOrder.AddRepairTask(repairTask);

            if (addResult.IsError)
            {
                logger.LogWarning(
                    "Could not add repair task {RepairTaskId} to work order {WorkOrderId}. Errors: {@Errors}",
                    repairTask.Id,
                    workOrder.Id,
                    addResult.Errors);

                return addResult.Errors;
            }
        }

        var updateTimingResult = workOrder.UpdateTiming(
            startAt,
            endAt);

        if (updateTimingResult.IsError)
        {
            logger.LogWarning(
                "Could not update timing for work order {WorkOrderId}. Errors: {@Errors}",
                workOrder.Id,
                updateTimingResult.Errors);

            return updateTimingResult.Errors;
        }

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Repair tasks and timing were updated for work order {WorkOrderId}.",
            request.WorkOrderId);

        return Result.Updated;
    }
}