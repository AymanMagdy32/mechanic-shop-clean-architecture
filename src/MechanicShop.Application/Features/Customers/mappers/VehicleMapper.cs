using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Entities.Customers.Vehicles;

public static class VehicleMapper
    {
        public static Result<Vehicle> ToEntity(this VehicleDto dto)
        {
            return Vehicle.Create(
                Guid.NewGuid(),
                dto.Make,
                dto.Model,
                dto.Year,
                dto.LicensePlate
            );
        }

        public static VehicleDto ToDto(this Vehicle vehicle)
        {
            return new VehicleDto
            {
                LicensePlate = vehicle.LicensePlate,
                Make = vehicle.Make,
                Model = vehicle.Model,
                Year = vehicle.Year
            };
        }
    }
