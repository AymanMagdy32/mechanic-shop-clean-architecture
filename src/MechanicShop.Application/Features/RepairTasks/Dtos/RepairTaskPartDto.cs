
namespace MechanicShop.Application.Features.RepairTasks.Commands.Dtos
{
    
 public sealed record RepairTaskPartDto
    {
        
    public string? Name { get;  set; }
    public decimal Cost { get;  set; }
    public int Quantity { get;  set; }
    }
    


}


//  public string Name { get; private set; }
//     public decimal LaborCost { get; private set; }
//     public RepairDurationInMinutes EstimatedDurationInMins { get; private set; }

//     private readonly List<Part> _parts = [];
//     public IEnumerable<Part> Parts => _parts.AsReadOnly();
