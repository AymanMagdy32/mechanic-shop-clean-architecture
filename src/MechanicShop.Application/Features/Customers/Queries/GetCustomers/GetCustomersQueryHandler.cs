using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Application.Features.Customers.GetCustomers;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Customers.Queries.GetCustomers
{
    public sealed class GetCustomersQueryHandler(
        ILogger<GetCustomersQueryHandler> logger,
        IAppDbContext context)
        : IRequestHandler<GetCustomersQuery, Result<List<CustomerDto>>>
    {
        public async Task<Result<List<CustomerDto>>> Handle(
            GetCustomersQuery request,
            CancellationToken cancellationToken)
        {
            var customers = await context.Customers
                .AsNoTracking()
                .Select(c => new CustomerDto
                {
                    Name = c.Name!,
                    Email = c.Email!,
                    PhoneNumber = c.PhoneNumber!,
                    Vehicles = c.Vehicles.Select(v => new VehicleDto
                    {
                        LicensePlate = v.LicensePlate,
                        Make = v.Make,
                        Model = v.Model,
                        Year = v.Year
                    }).ToList()
                })
                .ToListAsync(cancellationToken);

            if (customers.Count == 0)
            {
                logger.LogWarning("No customers were found.");

                return Error.NotFound(
                    code: "Customers_NotFound",
                    message: "No customers were found.");
            }

            return customers;
        }
    }
}