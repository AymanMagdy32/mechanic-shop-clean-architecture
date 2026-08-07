using FluentValidation;

namespace MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask;

public sealed class UpdateRepairTaskPartValidator : AbstractValidator<UpdateRepairTaskPartCommand>
{
	public UpdateRepairTaskPartValidator()
	{
		RuleFor(x => x.Quantity)
			.GreaterThan(0)
			.WithMessage("Quantity must be greater than zero.")
			.LessThanOrEqualTo(100)
			.WithMessage("Quantity must not exceed 100.");
	}
}
