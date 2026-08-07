using MechanicShop.Application.Common.ApplicationErrors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.Commands.RelocateWorkOrders
{
  public class RelocateWorkOrdersHandler(
    IAppDbContext context,
    HybridCache cache,
    ILogger<RelocateWorkOrdersHandler> logger) : IRequestHandler<RelocateWorkOrdersCommand, Result<Updated>>
  {
   public async Task<Result<Updated>> Handle(RelocateWorkOrdersCommand request, CancellationToken cancellationToken)
    {
      var workOrder = await context.WorkOrders
    .Include(w => w.RepairTasks)
    .Include(l=> l.Labor)
    .FirstOrDefaultAsync(
     w => w.Id == request.WorkOrderId,
     cancellationToken);

      if (workOrder == null)
      {
        logger.LogWarning($"WorkOrder with id : {request.WorkOrderId} not found.", request.WorkOrderId);
        return ApplicationErrors.WorkOrderNotFound;
      }

    var totalDurationInMinutes = workOrder.RepairTasks.Sum(rt => (int)rt.EstimatedDurationInMins);
    
   var endAt = request.NewStartAtUtc.AddMinutes(totalDurationInMinutes); 

   var isLaborOccupied = await context.WorkOrders
    .Where(w => w.LaborId == workOrder.LaborId && w.Id != workOrder.Id)
    .AnyAsync(w => w.StartAtUtc < endAt && w.EndAtUtc > request.NewStartAtUtc, cancellationToken);


      if (isLaborOccupied)
      {
        logger.LogWarning($"Labor with id : {workOrder.LaborId} is occupied during the requested time slot.", workOrder.LaborId);
        return ApplicationErrors.LaborOccupied;
      } 

      var isSpotOccupied = await context.WorkOrders
    .Where(w => w.Spot == request.NewSpot && w.Id != workOrder.Id)
    .AnyAsync(w => w.StartAtUtc < endAt && w.EndAtUtc > request.NewStartAtUtc, cancellationToken);

      if (isSpotOccupied)
      {
        logger.LogWarning($"Spot {request.NewSpot} is occupied during the requested time slot.", request.NewSpot);
        return ApplicationErrors.spotOccupied(request.NewStartAtUtc, endAt);
      }


   var updateSpotReslut =  workOrder.UpdateSpot(request.NewSpot);
   
      if (updateSpotReslut.IsError)
      {
        logger.LogWarning($"Failed to update WorkOrder spot. Id: {request.WorkOrderId}. Errors: {updateSpotReslut.Errors}", request.WorkOrderId, updateSpotReslut.Errors);
        return updateSpotReslut.Errors;
      }


   var UpdateTimingReslut =  workOrder.UpdateTiming(request.NewStartAtUtc, endAt); 

     if (UpdateTimingReslut.IsError)
      {
        logger.LogWarning($"Failed to update WorkOrder timing. Id: {request.WorkOrderId}. Errors: {UpdateTimingReslut.Errors}", request.WorkOrderId, UpdateTimingReslut.Errors);
        return UpdateTimingReslut.Errors;
      }
    
     

      await context.SaveChangesAsync(cancellationToken);

      await cache.RemoveAsync("work-orders", cancellationToken);

      return Result.Updated;
      
    }
  }

}