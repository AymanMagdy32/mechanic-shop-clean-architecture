using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Domain.Entities.RepairTasks;

namespace MechanicShop.Application.Features.RepairTasks.Mappers;

public static class RepairTaskMapper
{
    public static RepairTaskDto ToDto(this RepairTask entity)
    {
        return new RepairTaskDto
        {
            Id = entity.Id,
            Name = entity.Name,
            LaborCost = entity.LaborCost,
            EstimatedDuration = entity.EstimatedDurationInMins.ToString(),
            Parts = entity.Parts.Select(p => new PartDto
            {
                Id = p.Id,
                Name = p.Name ?? string.Empty,
                Cost = p.Cost,
                Quantity = p.Quantity
            }).ToList(),
            TotalCost = entity.TotalCost
        };
    }
}

