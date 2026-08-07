using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.WorkOrders.Dtos;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Features.WorkOrders.Queries.GetWorkOrderByIdQuery
{


    public sealed record GetWorkOrderByIdQuery(Guid WorkOrderId) : ICachedQuery<Result<WorkOrderDto>>
    {
        public string CacheKey => $"work-orders:{WorkOrderId}"; 

        public string[] Tags => ["work-orders"]; 

        public TimeSpan Expiration => TimeSpan.FromMinutes(10); 
    }





}