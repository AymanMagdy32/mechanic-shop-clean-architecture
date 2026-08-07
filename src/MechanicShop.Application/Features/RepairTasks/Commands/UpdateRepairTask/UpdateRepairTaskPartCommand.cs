using MediatR;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask;

public sealed record UpdateRepairTaskPartCommand(
    Guid PartId,
    string Name,
    decimal Cost,
    int Quantity) : IRequest<Result<Updated>>;
