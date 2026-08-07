using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Application.Features.RepairTasks.Mappers;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Entities.RepairTasks;
using MechanicShop.Domain.Entities.RepairTasks.Parts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask{

public sealed class CreateRepairTaskCommandHandler(
    IAppDbContext context,
    HybridCache cache,
    ILogger<CreateRepairTaskCommandHandler> logger)
    : IRequestHandler<CreateRepairTaskCommand, Result<RepairTaskDto>>
{
    public async Task<Result<RepairTaskDto>> Handle(CreateRepairTaskCommand request, CancellationToken ct)
    {
        var isExist = await context.RepairTasks
            .AnyAsync(x => x.Name == request.Name, ct);

        if (isExist)
        {
            logger.LogWarning("Repair task name already exists: {Name}", request.Name);

            return RepairTaskErrors.NameAlreadyExists; 
        }

        var parts = new List<Part>();

        foreach (var p in request.Parts ?? [])
        {
            var partResult = Part.Create(
                Guid.NewGuid(),
                p.Name!,
                p.Cost,
                p.Quantity);

            if (partResult.IsError)
            {
                logger.LogWarning("Failed to create part: {PartName}", p.Name);
                return partResult.Errors;
            }

            parts.Add(partResult.Value);
        }

        var repairTaskResult = RepairTask.Create(
            Guid.NewGuid(),
            request.Name,
            request.LaporCost,
            request.estimatedDurationInMins,
            parts);

        if (repairTaskResult.IsError)
        {
            logger.LogWarning("Failed to create repair task: {Name}", request.Name);
            return repairTaskResult.Errors;
        }

        context.RepairTasks.Add(repairTaskResult.Value);
        await context.SaveChangesAsync(ct);

    var dto = repairTaskResult.Value.ToDto();
        logger.LogInformation("A new Repair Task is Added succussfully {Name}:", request.Name ); 
        await cache.RemoveByTagAsync("RepairTask" , ct );

    return dto;

    }
}
}