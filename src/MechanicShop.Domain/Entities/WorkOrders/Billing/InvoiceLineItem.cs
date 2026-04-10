using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Entities.WorkOrders.Billing;

namespace MechanicShop.Domain.Entities.WorkOrders.Billing
{
    

public class InvoiceLineItem :AuditableEntity
    {
        public string? Description { get; private set; } 
        public Guid InvoiceId { get; private set; } 
        public int LineNumber { get; private set; } 
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; } 

        public decimal LineTotal => Quantity * UnitPrice;

   
     private InvoiceLineItem()
        { } 

        private InvoiceLineItem(Guid invoiceId, int lineNumber, string? description, int quantity, decimal unitPrice)
        {
            InvoiceId = invoiceId;
            LineNumber = lineNumber;
            Description = description;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public static Result<InvoiceLineItem> Create(
        Guid invoiceId,
        int lineNumber,
        string description,
        int quantity,
        decimal unitPrice)
    {
        if (invoiceId == Guid.Empty)
        {
            return InvoiceLineItemErrors.InvoiceIdRequired;
        }

        if (lineNumber <= 0)
        {
            return InvoiceLineItemErrors.LineNumberInvalid;
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return InvoiceLineItemErrors.DescriptionRequired;
        }

        if (quantity <= 0)
        {
            return InvoiceLineItemErrors.QuantityInvalid;
        }

        if (unitPrice <= 0)
        {
            return InvoiceLineItemErrors.UnitPriceInvalid;
        }

        return new InvoiceLineItem(invoiceId, lineNumber, description.Trim(), quantity, unitPrice);
    }

     
    }



}