using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.WorkOrders.Dtos;
using MechanicShop.Application.Features.WorkOrders.Mappers;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.Queries.GetWorkOrderByIdQuery;

public sealed class GetWorkOrderByIdQueryHandler(
    ILogger<GetWorkOrderByIdQueryHandler> logger,
    IAppDbContext context)
    : IRequestHandler<GetWorkOrderByIdQuery, Result<WorkOrderDto>>
{
    public async Task<Result<WorkOrderDto>> Handle(
        GetWorkOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var workOrder = await context.WorkOrders
            .AsNoTracking()
            .Include(x => x.Labor)
            .Include(x => x.Vehicle)
            .Include(x => x.RepairTasks)
                .ThenInclude(x => x.Parts)
            .FirstOrDefaultAsync(
                x => x.Id == request.WorkOrderId,
                cancellationToken);

        if (workOrder is null)
        {
            logger.LogWarning(
                "Work order with ID {WorkOrderId} was not found",
                request.WorkOrderId);

            return ApplicationErrors.WorkOrderNotFound;
        }

  
        logger.LogInformation(
            "Work order with ID {WorkOrderId} was retrieved successfully",
            request.WorkOrderId);

        return workOrder.ToDto();
    }
}