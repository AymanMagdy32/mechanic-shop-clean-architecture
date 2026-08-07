using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Labors.Query;
using MechanicShop.Domain.Common.Results.Abstractions;

namespace MechanicShop.Application.Feature.Labors.Query
{
   public sealed record GetLaborsQuery(): ICachedQuery<IResult<List<LaborDto>>>
    {
         public string CacheKey => $"labors"; 
         public string[] Tags => ["labors"]; 
         public TimeSpan Expiration => TimeSpan.FromMinutes(10);

    }  





}
 