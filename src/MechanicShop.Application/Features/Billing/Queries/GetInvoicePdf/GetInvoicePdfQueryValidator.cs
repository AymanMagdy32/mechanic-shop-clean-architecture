using FluentValidation;

namespace MechanicShop.Application.Features.Billing.GetInvoicePdf;

public sealed class InvoicePdfQueryValidator
    : AbstractValidator<InvoicePdfQuery>
{
    public InvoicePdfQueryValidator()
    {
        RuleFor(x => x.InvoiceId)
            .NotEmpty()
            .WithMessage("Invoice ID is required.");
    }
}