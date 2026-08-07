using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Entities.RepairTasks;
using MechanicShop.Domain.Entities.RepairTasks.Enums;
using MediatR;

namespace MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask
{
    
public sealed record CreateRepairTaskCommand(string Name , decimal LaporCost , RepairDurationInMinutes estimatedDurationInMins  , List<CreateRepairTaskPartCommmand>? Parts)
: IRequest<Result<RepairTaskDto>>
    {
        

    }




}









//  public string Name { get; private set; }
//     public decimal LaborCost { get; private set; }
//     public RepairDurationInMinutes EstimatedDurationInMins { get; private set; }

//     private readonly List<Part> _parts = [];
//     public IEnumerable<Part> Parts => _parts.AsReadOnly();
//     public decimal TotalCost => LaborCost + Parts.Sum(p => p.Cost * p.Quantity);
