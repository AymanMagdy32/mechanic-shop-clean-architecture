using FluentValidation;
using MechanicShop.Application.Features.Customers.Commands.UpdateCustomer;

namespace MechanicShop.Application.Features.Commands.UpdateCustomer
{

    public sealed class UpdateVechileCommandValidator :  AbstractValidator<UpdateVehicleCommand>
    {
        
 public UpdateVechileCommandValidator()
    {
        RuleFor(x => x.Make)
            .NotEmpty().MaximumLength(50);

        RuleFor(x => x.Model)
            .NotEmpty().MaximumLength(50);

        RuleFor(x => x.LicensePlate)
            .NotEmpty().MaximumLength(10);
    }


    }
    



}