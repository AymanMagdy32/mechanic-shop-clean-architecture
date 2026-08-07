namespace MechanicShop.Application.Labors.Query
{
    public sealed record LaborDto
{
    public Guid LaborId { get; set; }
    public string Name { get; set; } = string.Empty;
}
}
