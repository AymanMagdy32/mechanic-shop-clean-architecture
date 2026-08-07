using MechanicShop.Application.Common.ApplicationErrors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Entities.WorkOrders;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.Commands.RemoveWorkOrders
{
    
public sealed class RemoveWorkOrdersHandler(IAppDbContext context, ILogger<RemoveWorkOrdersHandler> logger, HybridCache cache) : IRequestHandler<RemoveWorkOrdersCommand, Result<Deleted>>
    {
      

        public async Task<Result<Deleted>> Handle(RemoveWorkOrdersCommand request, CancellationToken cancellationToken)
        {
            var workOrder = await context.WorkOrders.FindAsync(new object[] { request.WorkOrderId }, cancellationToken);

            if (workOrder == null)
            {
                logger.LogWarning($"WorkOrder with id : {request.WorkOrderId} not found.", request.WorkOrderId);
                return ApplicationErrors.WorkOrderNotFound;
            }

            context.WorkOrders.Remove(workOrder);
            await context.SaveChangesAsync(cancellationToken);
            
         await cache.RemoveAsync(
            "work-orders",
            cancellationToken);

          logger.LogInformation($"WorkOrder with id : {request.WorkOrderId} removed successfully ", workOrder!.Id);  

            return Result.Deleted; 
        }
    }



}