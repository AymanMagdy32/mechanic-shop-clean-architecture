using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.WorkOrders.Commands.RemoveWorkOrders
{
    
public sealed record RemoveWorkOrdersCommand(Guid WorkOrderId) : IRequest<Result<Deleted>>
{


}


}