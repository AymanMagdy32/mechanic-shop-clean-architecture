using FluentValidation;

namespace MechanicShop.Application.Features.WorkOrders.Commands.CreateWorkOrders
{
    public class CreateWorkOrderCommandValidator : AbstractValidator<CreateWorkOrderCommand>
    {
        public CreateWorkOrderCommandValidator()
        {
            RuleFor(x => x.Spot)
                .IsInEnum()
                .WithMessage("Invalid spot value.");

            RuleFor(x => x.VehicleId)
                .NotEmpty()
                .WithMessage("VehicleId is required.");

            RuleFor(x => x.StartAt)
                .GreaterThan(DateTimeOffset.Now)
                .WithMessage("StartAt must be in the future.");

            RuleFor(x => x.RepairTaskIds)
                .NotEmpty()
                .WithMessage("At least one RepairTaskId is required.");
        }

    
    }

}