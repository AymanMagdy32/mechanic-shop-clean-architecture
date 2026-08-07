using MechanicShop.Domain.Entities.WorkOrders.Billing;

namespace MechanicShop.Application.Common.Interfaces;

public interface IInvoicePdfGenerator
{
    byte[] Generate(Invoice invoice);
}