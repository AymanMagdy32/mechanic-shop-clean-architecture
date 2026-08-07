using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTasks;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTasks;

public sealed class GetRepairTasksQueryHandler(
    IAppDbContext context)
    : IRequestHandler<GetRepairTasksQuery, Result<List<RepairTaskDto>>>
{
    public async Task<Result<List<RepairTaskDto>>> Handle(GetRepairTasksQuery request, CancellationToken ct)
    {
        var items = await context.RepairTasks
            .AsNoTracking()
            .Select(rt => new RepairTaskDto
            {
                Id = rt.Id,
                Name = rt.Name,
                LaborCost = rt.LaborCost,
                EstimatedDuration = rt.EstimatedDurationInMins.ToString(),
                Parts = rt.Parts.Select(p => new PartDto
                {
                    Id = p.Id,
                    Name = p.Name ?? string.Empty,
                    Cost = p.Cost,
                    Quantity = p.Quantity
                }).ToList(),
                TotalCost = rt.LaborCost + rt.Parts.Sum(p => p.Cost * p.Quantity)
            })
            .ToListAsync(ct);

        if (items.Count == 0)
        {
            return Error.NotFound("RepairTasks.NotFound", "No repair tasks found.");
        }

        return items;
    }
}
