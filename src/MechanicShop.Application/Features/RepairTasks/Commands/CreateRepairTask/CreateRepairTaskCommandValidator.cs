using FluentValidation;
using MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask;

namespace MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask;

public sealed class CreateRepairTaskCommandValidator 
    : AbstractValidator<CreateRepairTaskCommand>
{
    public CreateRepairTaskCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Repair task name is required.")
            .MaximumLength(100)
            .WithMessage("Repair task name must not exceed 100 characters.");

        RuleFor(x => x.LaporCost)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Labor cost must be zero or greater.");

        RuleFor(x => x.estimatedDurationInMins)
            .IsInEnum()
            .WithMessage("Estimated duration is invalid.");

        RuleFor(x => x.Parts)
            .Must(parts => parts == null || parts.Count <= 50)
            .WithMessage("Repair task cannot contain more than 50 parts.");

        RuleForEach(x => x.Parts)
            .SetValidator(new CreateRepairTaskPartCommandValidator())
            .When(x => x.Parts is not null);
    }
}
