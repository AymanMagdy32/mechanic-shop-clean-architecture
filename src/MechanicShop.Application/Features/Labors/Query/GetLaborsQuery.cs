using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Labors.Query;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Features.Labors.Query;

public sealed record GetLaborsQuery
    : ICachedQuery<Result<List<LaborDto>>>
{
    public string CacheKey => "labors";

    public string[] Tags => ["labors"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}