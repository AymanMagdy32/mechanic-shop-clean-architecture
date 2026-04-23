namespace MechanicShop.Application.Features.Customers.Dtos
{
    public sealed record CustomerDto
    {
        public string Name { get;  set; } = string.Empty;
        public string Email { get;  set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public List<VehicleDto> Vehicles { get; set; } = []; 

    }
}