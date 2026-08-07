using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.WorkOrders.Commands.UpdateWorkOrdersRepairTasks
{
    
public sealed record UpdateWorkOrdersRepairTasksCommand(
    Guid WorkOrderId,
    Guid[] RepairTasksIds) : IRequest<Result<Updated>>
{


}

}