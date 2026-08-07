using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.WorkOrders.Commands.AssignLapor;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.Commands.AssignLabor;

public sealed class AssignLaborHandler(
    IAppDbContext context,
    ILogger<AssignLaborHandler> logger,
    HybridCache cache)
    : IRequestHandler<AssignLaborCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(
        AssignLaborCommand request,
        CancellationToken cancellationToken)
    {
        // Get WorkOrder
        var workOrder = await context.WorkOrders
            .FirstOrDefaultAsync(
                x => x.Id == request.WorkOrderId,
                cancellationToken);

        if (workOrder is null)
        {
            return ApplicationErrors.WorkOrderNotFound;
        }

        // Get Labor
        var labor = await context.Employees
            .FirstOrDefaultAsync(
                x => x.Id == request.LaborId,
                cancellationToken);

        if (labor is null)
        {
            return ApplicationErrors.LaborNotFound;
        }

    
        var isLaborOccupied = await context.WorkOrders.AnyAsync(
            x =>
                x.LaborId == request.LaborId &&
                x.Id != request.WorkOrderId &&
                x.StartAtUtc < workOrder.EndAtUtc &&
                x.EndAtUtc > workOrder.StartAtUtc,
            cancellationToken);

        if (isLaborOccupied)
        {
            return ApplicationErrors.LaborOccupied;
        }

      var updateResult =  workOrder.UpdateLabor(labor.Id); 

     if (updateResult.IsError)
{
 logger.LogWarning(
        "Failed to update Labor for WorkOrder {WorkOrderId}. Errors: {@Errors}",
        request.WorkOrderId,
        updateResult.Errors);  
          return updateResult.Errors;
}

        await context.SaveChangesAsync(cancellationToken);

     await cache.RemoveAsync(
            "work-orders",
            cancellationToken);
        logger.LogInformation(
            "Labor {LaborId} assigned to WorkOrder {WorkOrderId}",
            request.LaborId,
            request.WorkOrderId);

        return Result.Updated;
    }
}