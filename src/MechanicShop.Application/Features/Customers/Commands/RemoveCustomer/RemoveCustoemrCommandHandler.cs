using MechanicShop.Application.Common.ApplicationErrors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Entities.Customers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Customers.Commands.RemoveCustomer
{


    public sealed class RemoveCustomerCommandHandler(IAppDbContext context, ILogger<RemoveCustomerCommandHandler> logger, HybridCache cache) : IRequestHandler<RemoveCustomerCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(RemoveCustomerCommand request, CancellationToken ct)
        {
        var customer = await context.Customers.FirstOrDefaultAsync(c => c.Id == request.CustomerId);
 
          if (customer == null)
            {
                logger.LogWarning($"Customer with id : {request.CustomerId} not found.", request.CustomerId);
                return ApplicationErrors.CustomerNotFound;
            }

            var HasWorkOrders = await context.WorkOrders
            .Include(w => w.Vehicle)
            .Where(v => v.Vehicle != null)
            .AnyAsync(w => w.Vehicle!.CustomerId == request.CustomerId, ct);

            if (HasWorkOrders)
            {
                logger.LogWarning($"Customer with id : {request.CustomerId} has work orders associated with it. Consider archiving instead of deleting.", customer!.Id);
                return ApplicationErrors.CustomerHasAssociatedWorkOrders; 

            }
         
            context.Customers.Remove(customer);
            await context.SaveChangesAsync(ct) ;
   
            logger.LogInformation($"Customer with id : {request.CustomerId} removed successfully ", customer!.Id);

            await cache.RemoveByTagAsync("customers");

            return Result.Deleted;


        }
    }



}






