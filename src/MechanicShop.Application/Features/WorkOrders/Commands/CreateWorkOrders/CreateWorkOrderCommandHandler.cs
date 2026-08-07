using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.WorkOrders.Dtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Entities.WorkOrders;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.Commands.CreateWorkOrders
{
public sealed class CreateWorkOrderCommandHandler(
    IAppDbContext context,
    HybridCache cache,
    ILogger<CreateWorkOrderCommandHandler> logger)
    : IRequestHandler<CreateWorkOrderCommand, Result<WorkOrderDto>>
{
    public async Task<Result<WorkOrderDto>> Handle(
        CreateWorkOrderCommand request,
        CancellationToken cancellationToken)
    {
        if (request.RepairTaskIds is null ||
            request.RepairTaskIds.Count == 0)
        {
            return Error.Validation(
                code: "WorkOrder.RepairTasks.Required",
                message: "At least one repair task is required.");
        }

        if (request.LaborId is null ||
            request.LaborId.Value == Guid.Empty)
        {
            return Error.Validation(
                code: "WorkOrder.Labor.Required",
                message: "A labor employee is required.");
        }

        var repairTaskIds = request.RepairTaskIds
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        if (repairTaskIds.Count == 0)
        {
            return Error.Validation(
                code: "WorkOrder.RepairTasks.Required",
                message: "At least one valid repair task ID is required.");
        }

        var repairTasks = await context.RepairTasks
            .Where(repairTask =>
                repairTaskIds.Contains(repairTask.Id))
            .ToListAsync(cancellationToken);

        if (repairTasks.Count != repairTaskIds.Count)
        {
            var foundIds = repairTasks
                .Select(repairTask => repairTask.Id)
                .ToHashSet();

            var missingIds = repairTaskIds
                .Where(id => !foundIds.Contains(id))
                .ToList();

            logger.LogWarning(
                "One or more repair tasks were not found. Missing IDs: {@MissingRepairTaskIds}",
                missingIds);

            return ApplicationErrors.RepairTaskNotFound;
        }

        var totalDurationInMinutes = repairTasks.Sum(
            repairTask => (int)
                repairTask.EstimatedDurationInMins);

        if (totalDurationInMinutes <= 0)
        {
            logger.LogWarning(
                "Invalid total repair duration for repair tasks {@RepairTaskIds}.",
                repairTaskIds);

            return Error.Validation(
                code: "WorkOrder.Duration.Invalid",
                message: "The total repair-task duration must be greater than zero.");
        }

        var laborId = request.LaborId.Value;
        var endAt = request.StartAt.AddMinutes(
            totalDurationInMinutes);

        var vehicleExists = await context.Vehicles
            .AnyAsync(
                vehicle => vehicle.Id == request.VehicleId,
                cancellationToken);

        if (!vehicleExists)
        {
            logger.LogWarning(
                "Vehicle {VehicleId} was not found.",
                request.VehicleId);

            return ApplicationErrors.VehicleNotFound;
        }

        var laborExists = await context.Employees
            .AnyAsync(
                employee => employee.Id == laborId,
                cancellationToken);

        if (!laborExists)
        {
            logger.LogWarning(
                "Labor employee {LaborId} was not found.",
                laborId);

            return ApplicationErrors.LaborNotFound;
        }

        var isLaborOccupied = await context.WorkOrders
            .AnyAsync(
                workOrder =>
                    workOrder.LaborId == laborId &&
                    workOrder.StartAtUtc < endAt &&
                    workOrder.EndAtUtc > request.StartAt,
                cancellationToken);

        if (isLaborOccupied)
        {
            logger.LogWarning(
                "Labor {LaborId} is occupied between {StartAt} and {EndAt}.",
                laborId,
                request.StartAt,
                endAt);

            return ApplicationErrors.LaborOccupied;
        }

        var hasVehicleConflict = await context.WorkOrders
            .AnyAsync(
                workOrder =>
                    workOrder.VehicleId == request.VehicleId &&
                    workOrder.StartAtUtc < endAt &&
                    workOrder.EndAtUtc > request.StartAt,
                cancellationToken);

        if (hasVehicleConflict)
        {
            logger.LogWarning(
                "Vehicle {VehicleId} already has a work order between {StartAt} and {EndAt}.",
                request.VehicleId,
                request.StartAt,
                endAt);

            return ApplicationErrors.VehicleSchedulingConflict;
        }

        var isSpotOccupied = await context.WorkOrders
            .AnyAsync(
                workOrder =>
                    workOrder.Spot == request.Spot &&
                    workOrder.StartAtUtc < endAt &&
                    workOrder.EndAtUtc > request.StartAt,
                cancellationToken);

        if (isSpotOccupied)
        {
            logger.LogWarning(
                "Spot {Spot} is occupied between {StartAt} and {EndAt}.",
                request.Spot,
                request.StartAt,
                endAt);

            return Error.Conflict(
                code: "WorkOrder.Spot.Occupied",
                message: "The selected spot is occupied during the requested time.");
        }

        var createResult = WorkOrder.Create(
            id: Guid.NewGuid(),
            vehicleId: request.VehicleId,
            startAt: request.StartAt,
            endAt: endAt,
            laborId: laborId,
            spot: request.Spot,
            repairTasks: repairTasks);

        if (createResult.IsError)
        {
            logger.LogWarning(
                "Failed to create work order. Errors: {@Errors}",
                createResult.Errors);

            return createResult.Errors;
        }

        var workOrder = createResult.Value;

        context.WorkOrders.Add(workOrder);

        await context.SaveChangesAsync(cancellationToken);

        await cache.RemoveAsync(
            "work-orders",
            cancellationToken);

        logger.LogInformation(
            "Work order {WorkOrderId} was created for vehicle {VehicleId}.",
            workOrder.Id,
            workOrder.VehicleId);

        var dto = new WorkOrderDto(
            Id: workOrder.Id,
            Spot: workOrder.Spot.ToString(),
            VehicleId: workOrder.VehicleId,
            StartAt: workOrder.StartAtUtc,
            RepairTaskIds: workOrder.RepairTasks
                .Select(repairTask => repairTask.Id)
                .ToList(),
            LaborId: workOrder.LaborId,
            InvoiceStatus: workOrder.Invoice is null
                ? "NotIssued"
                : "Issued");

        return dto;
    }
}
}