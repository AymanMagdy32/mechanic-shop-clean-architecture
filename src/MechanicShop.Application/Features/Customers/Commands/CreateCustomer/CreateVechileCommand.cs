using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Domain.Common.Results;
using MediatR;

public sealed record CreateVechileCommand(string LicensePlate, string Make, string Model, int Year) : IRequest<Result<VehicleDto>>
    {
        
    }