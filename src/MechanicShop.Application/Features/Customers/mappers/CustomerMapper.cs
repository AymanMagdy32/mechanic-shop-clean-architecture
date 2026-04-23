using MechanicShop.Application.Features.Customers.Commands.CreateCustomer;
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Entities.Customers;
using MechanicShop.Domain.Entities.Customers.Vehicles;

namespace MechanicShop.Application.Features.Customers.Mappers
{
    public static class CustomerMapper
    {
        public static Result<Customer> ToEntity(this CreateCustomerCommand command)
        {
            var vehicles = new List<Vehicle>();

            if (command.Vehicles is not null && command.Vehicles.Count > 0)
            {
                foreach (var vehicleDto in command.Vehicles)
                {
                    var vehicleResult = Vehicle.Create(
                        Guid.NewGuid(),
                        vehicleDto.Make,
                        vehicleDto.Model,
                        vehicleDto.Year,
                        vehicleDto.LicensePlate
                    );

                    if (vehicleResult.IsError)
                    {
                        return vehicleResult.Errors;
                    }

                    vehicles.Add(vehicleResult.Value);
                }
            }

            var customerResult = Customer.Create(
                Guid.NewGuid(),
                command.Name,
                command.PhoneNumber,
                command.Email,
                vehicles
            );

            if (customerResult.IsError)
            {
                return customerResult.Errors;
            }

            return customerResult.Value;
        }

        public static CustomerDto ToDto(this Customer customer)
        {
            return new CustomerDto
            {
                Name = customer.Name ?? string.Empty,
                Email = customer.Email ?? string.Empty,
                PhoneNumber = customer.PhoneNumber ?? string.Empty,
                Vehicles = customer.Vehicles.Select(v => new VehicleDto
                {
                    LicensePlate = v.LicensePlate,
                    Make = v.Make,
                    Model = v.Model,
                    Year = v.Year
                }).ToList()
            };
        }
    }
}
