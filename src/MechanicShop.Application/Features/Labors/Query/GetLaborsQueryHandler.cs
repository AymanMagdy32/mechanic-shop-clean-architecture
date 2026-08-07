using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Labors.Mapper;
using MechanicShop.Application.Labors.Query;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Common.Results.Abstractions;
using MechanicShop.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Feature.Labors.Query
{

    public sealed class GetLaborsQueryHandler(
            ILogger<GetLaborsQueryHandler> logger,
            IAppDbContext context)
        : IRequestHandler<GetLaborsQuery, IResult<List<LaborDto>>>
    {
        public async Task<IResult<List<LaborDto>>> Handle(GetLaborsQuery request, CancellationToken cancellationToken)
        {
            var labors = await context.Employees.AsNoTracking().Where(a=> a.Role == Role.Labor).ToListAsync(cancellationToken);
            
            logger.LogInformation($"Retrieved {labors.Count} labors from the database.");
            return (IResult<List<LaborDto>>)labors.ToDtos();
        }
    }


}