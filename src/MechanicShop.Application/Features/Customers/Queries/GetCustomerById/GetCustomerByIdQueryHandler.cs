using System.Reflection.Metadata;
using MechanicShop.Application.Common.ApplicationErrors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Application.Features.Customers.Mappers;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Customers.Queries.GetCustomerById
{

   public sealed class GetCustomerByIdQueryHandler(
    ILogger<GetCustomerByIdQueryHandler> logger,
    IAppDbContext context
    )
    : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>
{
    public async Task<Result<CustomerDto>> Handle(GetCustomerByIdQuery query, CancellationToken ct)
    {
        var customer = await context.Customers.AsNoTracking().Include(c => c.Vehicles)
                                     .FirstOrDefaultAsync(c => c.Id == query.CustomerId, ct);

        if (customer is null)
        {
            logger.LogWarning("Customer with id {CustomerId} was not found", query.CustomerId);

            return Error.NotFound(
                code: "Customer_NotFound",
                message: $"Customer with id '{query.CustomerId}' was not found");
        }

        return customer.ToDto();
    }
}
}