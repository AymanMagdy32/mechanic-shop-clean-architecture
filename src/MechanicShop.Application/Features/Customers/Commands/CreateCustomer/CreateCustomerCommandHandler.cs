using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Application.Features.Customers.Mappers;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Entities.Customers;
using MechanicShop.Domain.Entities.Customers.Vehicles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Customers.Commands.CreateCustomer
{

    public sealed record CreateCustomerCommandHaandler(IAppDbContext Context, HybridCache Cache, Logger<CreateCustomerCommandHaandler> logger) : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
    {
        public async Task<Result<CustomerDto>> Handle(CreateCustomerCommand request, CancellationToken ct)
        {
            var email = request.Email.Trim().ToLower();

            var isExist = await Context.Customers.AnyAsync(c => c.Email!.Trim().ToLower() == email, ct);

            if (isExist)
            {
                logger.LogWarning("Customer with email {Email} already exists.", email);
                return CustomerErrors.EmailAlreadyExists;
            }

            var vehicle = new List<Vehicle>();

            foreach (var v in request.Vehicles)
            {
                var result = Vehicle.Create(Guid.NewGuid(), v.Make, v.Model, v.Year, v.LicensePlate);
                if (result.IsError)
                {
                    logger.LogWarning("Failed to create vehicle for customer {Email}. Error: {Error}", email, result.Errors);
                    return result.Errors;
                }
                vehicle.Add(result.Value);
            }

            var customerResult = Customer.Create(Guid.NewGuid(), request.Name.Trim(), request.PhoneNumber.Trim(), email, vehicle);
            if (customerResult.IsError)
            {
                logger.LogWarning("Failed to create customer with email {Email}. Error: {Error}", email, customerResult.Errors);
                return customerResult.Errors;
            }
            Context.Customers.Add(customerResult.Value);
            await Context.SaveChangesAsync(ct);

            logger.LogInformation($"Customer with email : {email} created successfully ", customerResult.Value.Id);

            await Cache.RemoveByTagAsync("customers");

            return customerResult.Value.ToDto();
        }

    }
}




