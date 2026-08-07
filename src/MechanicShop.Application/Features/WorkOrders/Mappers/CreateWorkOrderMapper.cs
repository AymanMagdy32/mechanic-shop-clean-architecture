using MechanicShop.Application.Features.WorkOrders.Dtos;
using MechanicShop.Domain.Entities.WorkOrders;

namespace MechanicShop.Application.Features.WorkOrders.Mappers;

public static class WorkOrderMapper
{
    public static WorkOrderDto ToDto(this WorkOrder workOrder)
    {
        ArgumentNullException.ThrowIfNull(workOrder);

        return new WorkOrderDto(
            workOrder.Id,
            workOrder.Spot.ToString(),
            workOrder.VehicleId,
            workOrder.StartAtUtc,
            workOrder.RepairTasks
                .Select(x => x.Id)
                .ToList(),
            workOrder.LaborId,
            workOrder.Invoice?.Status.ToString() ?? "Not Created");
    }
}