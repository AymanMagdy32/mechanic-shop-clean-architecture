using MechanicShop.Application.Common.ApplicationErrors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Billing.Dtos;
using MechanicShop.Application.Features.Billing.Mappers;
using MechanicShop.Domain.Common.Constamts;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Entities.WorkOrders.Billing;
using MechanicShop.Domain.Entities.WorkOrders.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Billing.Commands.IssueInvoice;

public sealed class IssueInvoiceCommandHandler(
    IAppDbContext context,
    HybridCache cache,
    ILogger<IssueInvoiceCommandHandler> logger,
    TimeProvider timeProvider)
    : IRequestHandler<IssueInvoiceCommand, Result<InvoiceDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly HybridCache _cache = cache;
    private readonly ILogger<IssueInvoiceCommandHandler> _logger = logger;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<Result<InvoiceDto>> Handle(
        IssueInvoiceCommand request,
        CancellationToken cancellationToken)
    {
        var workOrder = await _context.WorkOrders
            .Include(w => w.Vehicle!)
                .ThenInclude(v => v.Customer)
            .Include(w => w.RepairTasks)
                .ThenInclude(rt => rt.Parts)
            .FirstOrDefaultAsync(
                w => w.Id == request.WorkOrderId,
                cancellationToken);

        if (workOrder is null)
        {
            _logger.LogWarning(
                "WorkOrder not found. Id: {WorkOrderId}",
                request.WorkOrderId);

            return ApplicationErrors.WorkOrderNotFound;
        }

        if (workOrder.State != WorkOrderState.Completed)
        {
            _logger.LogWarning(
                "Invoice cannot be issued because WorkOrder {WorkOrderId} is not completed. Current state: {State}",
                workOrder.Id,
                workOrder.State);

            return ApplicationErrors.WorkOrderMustBeCompletedForInvoicing;
        }

        var invoiceAlreadyExists = await _context.Invoices
            .AnyAsync(
                invoice => invoice.WorkOrderId == workOrder.Id,
                cancellationToken);

        if (invoiceAlreadyExists)
        {
            _logger.LogWarning(
                "An invoice already exists for WorkOrder {WorkOrderId}",
                workOrder.Id);

            return ApplicationErrors.InvoiceAlreadyExistsForWorkOrder(workOrder.Id);
        }

        var invoiceId = Guid.NewGuid();
        var lineItems = new List<InvoiceLineItem>();

        var lineNumber = 1;

        foreach (var repairTask in workOrder.RepairTasks)
        {
            var totalPartsCost = repairTask.Parts.Sum(
                part => part.Cost * part.Quantity);

            var totalTaskCost =
                repairTask.LaborCost +
                totalPartsCost;

            var partsDescription = repairTask.Parts.Count != 0
                ? string.Join(
                    Environment.NewLine,
                    repairTask.Parts.Select(part =>
                        $"• {part.Name} x{part.Quantity} @ {part.Cost:F2}"))
                : "• No parts";

            var description =
                $"{repairTask.Name}{Environment.NewLine}" +
                $"Labor: {repairTask.LaborCost:F2}{Environment.NewLine}" +
                $"Parts:{Environment.NewLine}" +
                partsDescription;

            var createLineItemResult = InvoiceLineItem.Create(
                invoiceId: invoiceId,
                lineNumber: lineNumber,
                description: description,
                quantity: 1,
                unitPrice: totalTaskCost);

            if (createLineItemResult.IsError)
            {
                _logger.LogWarning(
                    "Failed to create line item for WorkOrder {WorkOrderId}. Errors: {@Errors}",
                    workOrder.Id,
                    createLineItemResult.Errors);

                return createLineItemResult.Errors;
            }

            lineItems.Add(createLineItemResult.Value);
            lineNumber++;
        }

        var subtotal = lineItems.Sum(item => item.LineTotal);

        const decimal taxRate = MechanicShopConstants.TaxRate; 

        var taxAmount = subtotal * taxRate;
        var discountAmount = workOrder.Discount ?? 0m;

        var createInvoiceResult = Invoice.Create(
            id: invoiceId,
            workOrderId: workOrder.Id,
            items: lineItems,
            discountAmount: discountAmount,
            taxAmount: taxAmount,
            datetime: _timeProvider);

        if (createInvoiceResult.IsError)
        {
            _logger.LogWarning(
                "Invoice creation failed for WorkOrder {WorkOrderId}. Errors: {@Errors}",
                workOrder.Id,
                createInvoiceResult.Errors);

            return createInvoiceResult.Errors;
        }

        var invoice = createInvoiceResult.Value;

        await _context.Invoices.AddAsync(
            invoice,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        await _cache.RemoveByTagAsync(
            "invoice",
            cancellationToken);

        _logger.LogInformation(
            "Invoice {InvoiceId} issued successfully for WorkOrder {WorkOrderId}",
            invoice.Id,
            workOrder.Id
            );

        return invoice.ToDto();
    }
}