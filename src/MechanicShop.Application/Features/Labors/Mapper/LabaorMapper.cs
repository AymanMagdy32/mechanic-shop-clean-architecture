using MechanicShop.Application.Labors.Query;
using MechanicShop.Domain.Employees;

namespace MechanicShop.Application.Features.Labors.Mapper
{
    public static class LaborMapper
    {
  public static LaborDto ToDto(this Employee employee)
    {
        return new LaborDto { LaborId = employee.Id, Name = employee.FullName };
    }

    public static List<LaborDto> ToDtos(this IEnumerable<Employee> entities)
    {
        return [.. entities.Select(l => l.ToDto())];
    }
}
}
