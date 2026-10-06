using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Labors.Mapper;
using MechanicShop.Application.Labors.Query;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Identity;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Labors.Query;

public sealed class GetLaborsQueryHandler(
    ILogger<GetLaborsQueryHandler> logger,
    IAppDbContext context)
    : IRequestHandler<GetLaborsQuery, Result<List<LaborDto>>>
{
    public async Task<Result<List<LaborDto>>> Handle(
        GetLaborsQuery request,
        CancellationToken cancellationToken)
    {
        var labors = await context.Employees
            .AsNoTracking()
            .Where(employee => employee.Role == Role.Labor)
            .ToListAsync(cancellationToken);

        logger.LogInformation(
            "Retrieved {LaborCount} labors from the database.",
            labors.Count);

        var laborDtos = labors.ToDtos();

        return laborDtos;
    }
}