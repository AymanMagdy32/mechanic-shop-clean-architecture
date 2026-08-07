using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Entities.RepairTasks;

public static class RepairTaskErrors
{

    public static Error InvalidRepairTaskId =>
        Error.Validation("RepairTask.Id.Invalid", "Invalid repair task ID.");
        public static Error notFound =>
        Error.NotFound("RepairTask.NotFound", "Repair task not found.");
    public static Error NameAlreadyExists =>
        Error.Conflict("RepairTask.Name.AlreadyExists", "A repair task with the same name already exists.");
    public static Error NameRequired =>
        Error.Validation("RepairTask.Name.Required", "Name is required.");

    public static Error LaborCostInvalid =>
        Error.Validation("RepairTask.LaborCost.Invalid", "Labor cost must be between 1 and 10,000.");

    public static Error DurationInvalid =>
        Error.Validation("RepairTask.Duration.Invalid", "Invalid duration selected.");

    public static Error PartsRequired =>
        Error.Validation("RepairTask.Parts.Required", "At least one part is required.");

    public static Error PartNameRequired =>
        Error.Validation("RepairTask.Parts.Name.Required", "All parts must have a name.");

    public static Error AtLeastOneRepairTaskIsRequired =>
          Error.Validation(
              code: "RepairTask.Required",
              message: "At least one repair task is required in a work order.");

    public static Error InUse =>
    Error.Conflict("RepairTask.InUse", "Cannot delete a repair task that is used in work orders.");

    public static Error DuplicateName =>

    Error.Conflict("RepairTaskPart.Duplicate", "A part with the same name already exists in this repair task.");
}