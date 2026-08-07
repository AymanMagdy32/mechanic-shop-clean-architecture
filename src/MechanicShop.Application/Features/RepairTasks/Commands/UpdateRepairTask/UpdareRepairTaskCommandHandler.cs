using MediatR;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Entities.RepairTasks;
using MechanicShop.Domain.Entities.RepairTasks.Parts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Hybrid;

namespace MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask;

public sealed class UpdateRepairTaskCommandHandler(
    IAppDbContext context,
    ILogger<UpdateRepairTaskCommandHandler> logger,
    HybridCache hybridCache)
    : IRequestHandler<UpdateRepairTaskCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(UpdateRepairTaskCommand request, CancellationToken ct)
    {
        logger.LogInformation(
            "Updating RepairTask. Id: {RepairTaskId}",
            request.RepairTaskId);

        var existing = await context.RepairTasks
            .Include(x => x.Parts)
            .FirstOrDefaultAsync(x => x.Id == request.RepairTaskId, ct);

        if (existing is null)
        {
            logger.LogWarning(
                "RepairTask not found. Id: {RepairTaskId}",
                request.RepairTaskId);

            return RepairTaskErrors.notFound;
        }

        var updateResult = existing.Update(
            request.Name,
            request.LaborCost,
            request.EstimatedDurationInMins);

        if (updateResult.IsError)
        {
            logger.LogWarning(
                "Failed to update RepairTask. Id: {RepairTaskId}. Errors: {@Errors}",
                request.RepairTaskId,
                updateResult.Errors);

            return updateResult.Errors;
        }

        if (request.Parts is not null)
        {
            existing.ClearParts();

            foreach (var p in request.Parts)
            {
                var partResult = Part.Create(
                    p.PartId,
                    p.Name,
                    p.Cost,
                    p.Quantity);

                if (partResult.IsError)
                {
                    logger.LogWarning(
                        "Failed to create Part while updating RepairTask. RepairTaskId: {RepairTaskId}. Errors: {@Errors}",
                        request.RepairTaskId,
                        partResult.Errors);

                    return partResult.Errors;
                }
                existing.AddPart(partResult.Value);
            }
        }

        await context.SaveChangesAsync(ct);

        await hybridCache.RemoveAsync($"repairtask_{request.RepairTaskId}", ct);
        await hybridCache.RemoveByTagAsync("repairtask", ct);

        logger.LogInformation(
            "RepairTask updated successfully. Id: {RepairTaskId}",
            request.RepairTaskId);

        return Result.Updated;
    }
    
}