namespace MechanicShop.Application.Features.Customers.Dtos
{
    public sealed record VehicleDto
    {
        public string LicensePlate { get;  set; } = string.Empty;
        public string Make { get;  set; } = string.Empty;
        public string Model { get;  set; } = string.Empty;
        public int Year { get;  set; }
    }


}