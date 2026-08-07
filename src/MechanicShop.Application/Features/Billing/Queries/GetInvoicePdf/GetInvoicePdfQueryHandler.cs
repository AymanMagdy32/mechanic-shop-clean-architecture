using MechanicShop.Application.Common.ApplicationErrors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Billing.Dtos;
using MechanicShop.Application.Features.Billing.GetInvoicePdf;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Entities.WorkOrders.Billing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Application.Features.Billing.InvoicePdf;

public sealed class InvoicePdfQueryHandler
    : IRequestHandler<InvoicePdfQuery, Result<InvoicePdfDto>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IInvoicePdfGenerator _invoicePdfService;

    public InvoicePdfQueryHandler(
        IAppDbContext dbContext,
        IInvoicePdfGenerator invoicePdfService)
    {
        _dbContext = dbContext;
        _invoicePdfService = invoicePdfService;
    }

    public async Task<Result<InvoicePdfDto>> Handle(
        InvoicePdfQuery request,
        CancellationToken cancellationToken)
    {
        Invoice? invoice = await _dbContext.Invoices
            .AsNoTracking()
            .Include(x => x.LineItems)
            .FirstOrDefaultAsync(
                x => x.Id == request.InvoiceId,
                cancellationToken);

        if (invoice is null)
        {
            return ApplicationErrors.InvoiceNotFound; 
        }

        byte[] pdf = _invoicePdfService.Generate(invoice);

       return new InvoicePdfDto
{
    Content = pdf,
    FileName = $"Invoice-{invoice.Id}.pdf",
    ContentType = "application/pdf"
};
    }
}