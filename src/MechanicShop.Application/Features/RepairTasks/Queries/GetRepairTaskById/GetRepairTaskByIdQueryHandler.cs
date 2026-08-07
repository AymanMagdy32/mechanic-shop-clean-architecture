using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTaskById;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MechanicShop.Application.Features.RepairTasks.Mappers;

namespace MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTaskById;

public sealed class GetRepairTaskByIdQueryHandler(
    IAppDbContext context)
    : IRequestHandler<GetRepairTaskByIdQuery, Result<RepairTaskDto>>
{
    public async Task<Result<RepairTaskDto>> Handle(GetRepairTaskByIdQuery request, CancellationToken ct)
    {
        var entity = await context.RepairTasks
            .AsNoTracking()
            .Include(rt => rt.Parts)
            .FirstOrDefaultAsync(rt => rt.Id == request.RepairTaskId, ct);

        if (entity is null)
        {
            return Error.NotFound("RepairTask.NotFound", "Repair task not found.");
        }

        var dto = entity.ToDto();

        return dto;
    }
}
