using System.Runtime.CompilerServices;
using MechanicShop.Application.Common.ApplicationErrors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Entities.Customers.Vehicles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Customers.Commands.UpdateCustomer;

public sealed class UpdateCustomerCommandHandler(
    IAppDbContext context,
    ILogger<UpdateCustomerCommandHandler> logger,
    HybridCache cache)
    : IRequestHandler<UpdateCustomerCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(UpdateCustomerCommand request, CancellationToken ct)
    {
        var customer = await context.Customers
            .Include(c => c.Vehicles)
            .FirstOrDefaultAsync(c => c.Id == request.CustomerId, ct);

        if (customer is null)
        {
            logger.LogWarning("Customer with id {CustomerId} was not found.", request.CustomerId);
            return ApplicationErrors.CustomerNotFound;
        }

        var updateResult = customer.Update(request.Name, request.Email, request.PhoneNumber);

        if (updateResult.IsError)
        {
            logger.LogWarning(
                "Failed to update customer with id {CustomerId}. Errors: {@Errors}",
                request.CustomerId,
                updateResult.Errors);

            return updateResult.Errors;
        }

        var incomingVehicles = new List<Vehicle>();

        foreach (var vehicleRequest in request.Vehicles)
        {
         var vehicleId = vehicleRequest.VehicleId ?? Guid.NewGuid(); 

         var vehicleResult = Vehicle.Create(
                vehicleId,
                vehicleRequest.Make,
                vehicleRequest.Model,
                vehicleRequest.Year,
                vehicleRequest.LicensePlate);

            if (vehicleResult.IsError)
            {
                logger.LogWarning(
                    "Failed to create or update vehicle for customer {CustomerId}. Errors: {@Errors}",
                    request.CustomerId,
                    vehicleResult.Errors);

                return vehicleResult.Errors;
            }

            incomingVehicles.Add(vehicleResult.Value);

        }

        var upsertVehiclesResult = customer.UpsertVehicles(incomingVehicles);

        if (upsertVehiclesResult.IsError)
        {
            logger.LogWarning(
                "Failed to upsert vehicles for customer {CustomerId}. Errors: {@Errors}",
                request.CustomerId,
                upsertVehiclesResult.Errors);

            return upsertVehiclesResult.Errors;
        }

        await context.SaveChangesAsync(ct);
        await cache.RemoveByTagAsync("customers", ct);

        return Result.Updated;
    }
}