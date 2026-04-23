using System.Security.Cryptography.X509Certificates;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Entities.Customers
{
   public static class CustomerErrors
    {
        public static Error NameRequired => Error.Validation("Customer.Name.Required", "Customer name is required.");
        public static Error EmailInvalid => Error.Validation("Customer.Email.Invalid", "Customer email is invalid.");
        public static Error EmailAlreadyExists => Error.Conflict("Customer.Email.Exists", "Customer with the same email already exists.");
        public static Error EmailRequired => Error.Validation("Customer.Email.Required", "Customer email is required.");
        public static Error PhoneNumberRequired => Error.Validation("Customer.PhoneNumber.Required", "Customer phone number is required.");
        public static Error InvalidEmail => Error.Validation("Customer.Email.Invalid", "Customer email is invalid.");
        public static Error InvalidPhoneNumber => Error.Validation("Customer.PhoneNumber.Invalid", "Customer phone number is invalid.");
        public static readonly Error CannotDeleteCustomerWithWorkOrders = Error.Conflict("Customer.Delete.Conflict", "Cannot delete customer with existing work orders.");
    }

}