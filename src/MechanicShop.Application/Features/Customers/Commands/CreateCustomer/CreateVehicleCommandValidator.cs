using FluentValidation;

namespace MechanicShop.Application.Features.Customers.Commands.CreateCustomer
{
    public sealed class CreateVehicleCommandValidator : AbstractValidator<CreateVechileCommand>
    {
        public CreateVehicleCommandValidator()
        {
            RuleFor(x => x.Make)
                .NotEmpty().WithMessage("Make is required.")
                .MaximumLength(50).WithMessage("Make cannot exceed 50 characters.");

            RuleFor(x => x.Model)
                .NotEmpty().WithMessage("Model is required.")
                .MaximumLength(50).WithMessage("Model cannot exceed 50 characters.");

            RuleFor(x => x.Year)
                .InclusiveBetween(1886, DateTime.Now.Year + 1).WithMessage($"Year must be between 1886 and {DateTime.Now.Year + 1}.");

            RuleFor(x => x.LicensePlate)
                .NotEmpty().WithMessage("License plate is required.")
                .MaximumLength(20).WithMessage("License plate cannot exceed 20 characters.");
        }
    }
}