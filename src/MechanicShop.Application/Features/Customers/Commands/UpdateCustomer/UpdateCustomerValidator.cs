using System.Data;
using FluentValidation;
using MechanicShop.Application.Features.Customers.Commands.CreateCustomer;

namespace MechanicShop.Application.Features.Customers.Commands.UpdateCustomer
{

  public sealed class UpdateCustomerValidator : AbstractValidator<UpdateCustomerCommand>
    { 
        public UpdateCustomerValidator()
        {
              RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email")
            .MaximumLength(100);

        RuleFor(x => x.PhoneNumber)
         .NotEmpty().WithMessage("Phone number is required.")
         .Matches(@"^\+?\d{7,15}$").WithMessage("Phone number must be 7–15 digits and may start with '+'.");

        RuleFor(x => x.Vehicles)
            .NotNull().WithMessage("Vehicle list cannot be null.")
            .Must(p => p.Count > 0).WithMessage("At least one vehicle is required.");

            


        }
        

    }






}