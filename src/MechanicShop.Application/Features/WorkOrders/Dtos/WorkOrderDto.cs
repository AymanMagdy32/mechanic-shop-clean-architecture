namespace MechanicShop.Application.Features.WorkOrders.Dtos
{
    public sealed record WorkOrderDto(
        Guid Id,
        string Spot,
        Guid VehicleId,
        DateTimeOffset StartAt,
        List<Guid> RepairTaskIds,
        Guid? LaborId,
        string InvoiceStatus);
}