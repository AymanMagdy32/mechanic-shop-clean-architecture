using MechanicShop.Application.Features.RepairTasks.Commands.Dtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Entities.RepairTasks;
using MechanicShop.Domain.Entities.RepairTasks.Parts;

namespace MechanicShop.Application.Features.RepairTasks.Commands.Mapper
{
    


public static class RepairTaskPartMapper
{


    public static Result<RepairTaskPartDto> ToDto(this Part part)
        {
            return new RepairTaskPartDto
            {
                Name = part.Name, Cost = part.Cost , Quantity = part.Quantity 
                
            };
        }

}




}