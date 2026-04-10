using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Entities.WorkOrders.Enums;

namespace MechanicShop.Domain.Entities.WorkOrders;

public static class WorkOrderErrors
{
    public static Error WorkOrderIdRequired => Error.Validation(
        code: "WorkOrderErrors.WorkOrderIdRequired",
        message: "WorkOrder Id is required");

    public static Error VehicleIdRequired => Error.Validation(
        code: "WorkOrderErrors.VehicleIdRequired",
        message: "Vehicle Id is required");
    public static Error RepairTasksRequired => Error.Validation(
        code: "WorkOrderErrors.RepairTasksRequired",
        message: "At least one repair task is required");

    public static Error LaborIdRequired => Error.Validation(
        code: "WorkOrderErrors.LaborIdRequired",
        message: "Labor Id is required");
    public static Error InvalidTiming => Error.Conflict(
        code: "WorkOrderErrors.InvalidTiming",
        message: "End time must be after start time.");

    public static Error SpotInvalid => Error.Validation(
        code: "WorkOrderErrors.SpotInvalid",
        message: "The provided spot is invalid");
    public static Error Readonly => Error.Conflict(
        code: "WorkOrderErrors.Readonly",
        message: "WorkOrder is read-only.");

    public static Error TimingReadonly(string id, WorkOrderState state) => Error.Conflict(
        code: "WorkOrderErrors.TimingReadonly",
        message: $"WorkOrder '{id}': Can't Modify timing when WorkOrder status is '{state}'.");
    public static Error LaborIdEmpty(string id) => Error.Validation(
        code: "WorkOrderErrors.LaborIdEmpty",
        message: $"WorkOrder '{id}': Labor Id is empty");

    public static Error StateTransitionNotAllowed(DateTimeOffset startAtUtc) => Error.Conflict(
       code: "WorkOrderErrors.StateTransitionNotAllowed",
       message: $"State transition is not allowed before the work order’s scheduled start time {startAtUtc:yyyy-MM-dd HH:mm} UTC.");
    public static Error InvalidStateTransition(WorkOrderState current, WorkOrderState next) => Error.Conflict(
        code: "WorkOrderErrors.InvalidStateTransition",
        message: $"WorkOrder Invalid State transition from '{current}' to '{next}'.");

    public static Error RepairTaskAlreadyAdded => Error.Conflict(
        code: "WorkOrderErrors.RepairTaskAlreadyAdded",
        message: "Repair task already exists.");
    public static Error InvalidStateTransitionTime => Error.Conflict(
        code: "WorkOrderErrors.InvalidStateTransitionTime",
        message: "State transition is not allowed before the work order’s scheduled start time.");
}