using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Entities.WorkOrders.Billing;

public static class InvoiceErrors
{
    public static readonly Error WorkOrderIdInvalid = Error.Validation(
        code: "Invoice.WorkOrderId.Invalid",
        message: "WorkOrderId is invalid");

    public static readonly Error LineItemsEmpty = Error.Validation(
        code: "Invoice.LineItems.Empty",
        message: "Invoice must have line items");
    public static readonly Error InvoiceLocked = Error.Validation(
        code: "Invoice.Locked",
        message: "Invoice is locked");

    public static readonly Error DiscountNegative = Error.Validation(
        code: "Invoice.Discount.Negative",
        message: "Discount cannot be negative");
    public static readonly Error DiscountExceedsSubtotal = Error.Validation(
        code: "Invoice.Discount.ExceedsSubtotal",
        message: "Discount exceeds subtotal");
}