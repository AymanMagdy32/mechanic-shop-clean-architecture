namespace MechanicShop.Application.Features.RepairTasks.Dtos;

public sealed record RepairTaskDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal LaborCost { get; set; }
    public string EstimatedDuration { get; set; } = string.Empty;
    public List<PartDto> Parts { get; set; } = new();
    public decimal TotalCost { get; set; }
}

