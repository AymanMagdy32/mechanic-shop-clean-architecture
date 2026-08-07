using MechanicShop.Application.Features.Billing.Dtos;
using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.Billing.GetInvoicePdf;

public sealed record InvoicePdfQuery(Guid InvoiceId)
    : IRequest<Result<InvoicePdfDto>>;