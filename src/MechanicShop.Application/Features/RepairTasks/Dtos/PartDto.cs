namespace MechanicShop.Application.Features.RepairTasks.Dtos;

public sealed record PartDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public int Quantity { get; set; }
}
