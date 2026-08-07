using FluentValidation;

namespace MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask{

public sealed class CreateRepairTaskPartCommandValidator 
    : AbstractValidator<CreateRepairTaskPartCommmand>
{
    public CreateRepairTaskPartCommandValidator()
    {

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.")
            .LessThanOrEqualTo(100)
            .WithMessage("Quantity must not exceed 100.");
    }
}
}