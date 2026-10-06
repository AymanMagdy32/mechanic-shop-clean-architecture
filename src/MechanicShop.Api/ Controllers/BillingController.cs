using MechanicShop.Application.Feature.Billing.SettleInvoice;
using MechanicShop.Application.Features.Billing.Commands.IssueInvoice;
using MechanicShop.Application.Features.Billing.Dtos;
using MechanicShop.Application.Features.Billing.GetInvoicePdf;
using MechanicShop.Application.Features.Billing.Queries.GetInvoiceById;

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MechanicShop.Api.Controllers;

[ApiController]
[Route("api/billing")]
public sealed class BillingController(ISender sender) : ControllerBase
{
    // POST: api/billing/work-orders/{workOrderId}/invoice
    [HttpPost("work-orders/{workOrderId:guid}/invoice")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> IssueInvoice(
        Guid workOrderId,
        CancellationToken ct)
    {
        var command = new IssueInvoiceCommand(workOrderId);

        var result = await sender.Send(command, ct);

        if (result.IsError)
        {
            return BadRequest(result.Errors);
        }

        return StatusCode(
            StatusCodes.Status201Created,
            result.Value);
    }


    // GET: api/billing/invoices/{invoiceId}
    [HttpGet("invoices/{invoiceId:guid}")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInvoiceById(
        Guid invoiceId,
        CancellationToken ct)
    {
        var query = new GetInvoiceByIdQuery(invoiceId);

        var result = await sender.Send(query, ct);

        if (result.IsError)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }


    // POST: api/billing/invoices/{invoiceId}/settle
    [HttpPost("invoices/{invoiceId:guid}/settle")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SettleInvoice(
        Guid invoiceId,
        CancellationToken ct)
    {
        var command = new SettleInvoiceCommand(invoiceId);

        var result = await sender.Send(command, ct);

        if (result.IsError)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }


    // GET: api/billing/invoices/{invoiceId}/pdf
    [HttpGet("invoices/{invoiceId:guid}/pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInvoicePdf(
        Guid invoiceId,
        CancellationToken ct)
    {
        var query = new InvoicePdfQuery(invoiceId);

        var result = await sender.Send(query, ct);

        if (result.IsError)
        {
            return NotFound(result.Errors);
        }

        var pdf = result.Value;

        if (pdf.Content is null || pdf.Content.Length == 0)
        {
            return NotFound(new
            {
                Message = "Invoice PDF could not be generated."
            });
        }

        return File(
            pdf.Content,
            pdf.ContentType ?? "application/pdf",
            pdf.FileName ?? $"Invoice-{invoiceId}.pdf");
    }
}