
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Entities.WorkOrders.Billing;

public static class InvoiceLineItemErrors
{
    public static Error InvoiceIdRequired => Error.Validation(
        code: "InvoiceLineItemErrors.InvoiceIdRequired",
        message: "InvoiceId is required.");

    public static Error LineNumberInvalid => Error.Validation(
        code: "InvoiceLineItemErrors.LineNumberInvalid",
        message: "Line number must be greater than 0.");
    public static Error DescriptionRequired => Error.Validation(
        code: "InvoiceLineItemErrors.DescriptionRequired",
        message : "Description is required.");

    public static Error QuantityInvalid => Error.Validation(
        code: "InvoiceLineItemErrors.QuantityInvalid",
        message: "Quantity must be greater than 0.");
    public static Error UnitPriceInvalid => Error.Validation(
        code: "InvoiceLineItemErrors.UnitPriceInvalid",
        message : "Unit price must be greater than 0.");
}