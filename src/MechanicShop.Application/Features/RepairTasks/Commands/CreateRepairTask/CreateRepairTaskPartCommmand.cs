using MechanicShop.Application.Features.RepairTasks.Commands.Dtos;
using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask
{
    
public sealed record CreateRepairTaskPartCommmand(string? Name , decimal Cost , int Quantity ): IRequest<Result<RepairTaskPartDto>>
    {
    
        
    }


}