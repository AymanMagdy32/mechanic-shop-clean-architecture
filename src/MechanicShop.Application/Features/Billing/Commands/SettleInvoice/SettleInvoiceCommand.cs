using MechanicShop.Application.Features.Billing.Dtos;
using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Feature.Billing.SettleInvoice; 


public sealed record SettleInvoiceCommand(
    Guid InvoiceId
) : IRequest<Result<Success>>;