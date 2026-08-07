using MechanicShop.Application.Common.ApplicationErrors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Entities.WorkOrders;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.Commands.UpdateOrderState;

public sealed class UpdateOrderStateHandler(
    IAppDbContext context,
    ILogger<UpdateOrderStateHandler> logger,
    HybridCache cache)
    : IRequestHandler<UpdateOrderStateCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(
        UpdateOrderStateCommand request,
        CancellationToken cancellationToken)
    {
        var workOrder = await context.WorkOrders.FindAsync(
            [request.WorkOrderId],
            cancellationToken);

        if (workOrder is null)
        {
            logger.LogWarning(
                "WorkOrder with id {WorkOrderId} was not found.",
                request.WorkOrderId);

            return ApplicationErrors.WorkOrderNotFound;
        }

        workOrder.UpdateState(request.NewState);

        await context.SaveChangesAsync(cancellationToken);
        await cache.RemoveAsync("work-orders", cancellationToken);

        logger.LogInformation(
            "WorkOrder with id {WorkOrderId} was updated successfully.",
            workOrder.Id);

        return Result.Updated;
    }
}