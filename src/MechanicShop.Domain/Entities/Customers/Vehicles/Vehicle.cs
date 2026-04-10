using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Entities.Customers.Vehicles;

namespace MechanicShop.Domain.Entities.Customers.Vehicles
{
public sealed class Vehicle : AuditableEntity
{
    public Guid CustomerId { get; private set; }
    public string LicensePlate { get; private set; } = string.Empty; 
    public string Make { get; private set; } = string.Empty; 
    public string Model { get; private set; } = string.Empty;
    public int Year { get; private set; }

     public string VechicleInfo => $"{Make} | {Model} | {Year}"; 


    private Vehicle()    {} 

    private Vehicle(Guid id, Guid customerId, string licensePlate, string make, string model, int year) : base(id)
    {
        CustomerId = customerId;
        LicensePlate = licensePlate;
        Make = make;
        Model = model;
        Year = year;
    }
  public static Result<Vehicle> Create(Guid id, Guid customerId, string licensePlate, string make, string model, int year)
    {
        if (string.IsNullOrWhiteSpace(licensePlate))
            return VehicleErrors.LicensePlateRequired;
        if (string.IsNullOrWhiteSpace(make))
            return VehicleErrors.MakeRequired;
        if (string.IsNullOrWhiteSpace(model))
            return VehicleErrors.ModelRequired;
        if (year <= 0)
            return VehicleErrors.InvalidYear;

        return new Vehicle(id, customerId, licensePlate, make, model, year);



}
  public Result<Updated> Update(string make, string model, int year, string licensePlate)
    {
        if (string.IsNullOrWhiteSpace(make))
        {
            return VehicleErrors.MakeRequired;
        }

        if (string.IsNullOrWhiteSpace(model))
        {
            return VehicleErrors.ModelRequired;
        }

        if (year < 1886 || year > DateTime.UtcNow.Year)
        {
            return VehicleErrors.InvalidYear;
        }

        if (string.IsNullOrWhiteSpace(licensePlate))
        {
            return VehicleErrors.LicensePlateRequired;
        }

        Make = make;
        Model = model;
        Year = year;
        LicensePlate = licensePlate;

        return Result.Updated;
    }
}
}