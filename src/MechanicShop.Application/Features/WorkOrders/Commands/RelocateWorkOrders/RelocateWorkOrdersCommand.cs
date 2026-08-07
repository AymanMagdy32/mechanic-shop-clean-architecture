using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Entities.WorkOrders.Enums;
using MediatR;

namespace MechanicShop.Application.Features.WorkOrders.Commands.RelocateWorkOrders
{
 public sealed record RelocateWorkOrdersCommand(
    Guid WorkOrderId,
    DateTime NewStartAtUtc,
    Spot NewSpot ) : IRequest<Result<Updated>>{}

    
}