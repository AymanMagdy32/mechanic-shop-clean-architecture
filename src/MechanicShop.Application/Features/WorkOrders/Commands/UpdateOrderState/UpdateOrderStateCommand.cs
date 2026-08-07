using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Entities.WorkOrders.Enums;
using MediatR;

namespace MechanicShop.Application.Features.WorkOrders.Commands.UpdateOrderState
{
    
public sealed record UpdateOrderStateCommand(
    Guid WorkOrderId,
    WorkOrderState NewState) : IRequest<Result<Updated>>
{



}
}