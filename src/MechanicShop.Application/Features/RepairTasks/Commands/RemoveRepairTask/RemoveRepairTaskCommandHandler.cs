using MediatR;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Entities.RepairTasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Hybrid;

namespace MechanicShop.Application.Features.RepairTasks.Commands.RemoveRepairTask;

public sealed class RemoveRepairTaskCommandHandler(
    IAppDbContext context,
    ILogger<RemoveRepairTaskCommandHandler> logger,
    HybridCache cache)
    : IRequestHandler<RemoveRepairTaskCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(RemoveRepairTaskCommand request, CancellationToken ct)
    {
        logger.LogInformation("Removing RepairTask. Id: {RepairTaskId}", request.RepairTaskId);

        var existing = await context.RepairTasks
            .Include(x => x.Parts)
            .FirstOrDefaultAsync(x => x.Id == request.RepairTaskId, ct);

        if (existing is null)
        {
            logger.LogWarning("RepairTask not found. Id: {RepairTaskId}", request.RepairTaskId);
            return RepairTaskErrors.notFound;
        }

        // If the repair task is used in work orders, return conflict
        var inUse = await context.WorkOrders.AnyAsync(wo => wo.RepairTasks.Any(rt => rt.Id == request.RepairTaskId), ct);
        if (inUse)
        {
            logger.LogWarning("Cannot remove RepairTask in use. Id: {RepairTaskId}", request.RepairTaskId);
            return RepairTaskErrors.InUse;
        }

        context.RepairTasks.Remove(existing);
        await context.SaveChangesAsync(ct);

        await cache.RemoveByTagAsync("repairtask", ct);

        logger.LogInformation("RepairTask removed successfully. Id: {RepairTaskId}", request.RepairTaskId);

        return Result.Updated;
    }
}
