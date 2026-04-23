using FluentValidation;

namespace MechanicShop.Application.Features.Customers.Commands.RemoveCustomer
{
    
 public sealed class RemoveCustomerValidator : AbstractValidator<RemoveCustomerCommand>
    {

        public RemoveCustomerValidator()
        {

        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer Id is required.");
            
        }
        


    }




}